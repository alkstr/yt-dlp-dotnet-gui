using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using YTDLP.Dotnet.GUI.Assets;
using YTDLP.Dotnet.GUI.Models;
using YTDLP.Dotnet.GUI.Utilities;

namespace YTDLP.Dotnet.GUI.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    public LocalMediaFile Media { get; } = new();
    public Logger Logger { get; } = new();

    public enum EditError
    {
        NoFFmpeg,
        NoFile,
        AnotherInProgress,
        Other,
    }

    public enum MetadataError
    {
        NoFFProbe,
        NoFile,
        Other,
    }

    public async Task<Error<EditError>?> EditAsync()
    {
        if (!File.Exists(Configuration.FFmpegPath))
        {
            return new(EditError.NoFFmpeg, Resources.NoFFmpeg_Edit_Error);
        }
        if (!File.Exists(Media.Path))
        {
            return new(EditError.NoFile, Resources.NoFile_Edit_Error);
        }
        if (Monitor.IsEntered(editLock))
        {
            return new(EditError.AnotherInProgress, Resources.AnotherInProgress_Edit_Error);
        }

        try
        {
            Monitor.Enter(editLock);
            Logger.Clear();

            var process = FFmpeg.Edit.Process(
                Configuration.FFmpegPath,
                Media.Path,
                Media.Cut ? (Media.CutStart.FullString, Media.CutEnd.FullString) : (null, null),
                (Media.ChangeWidth ? (uint?)Media.NewWidth : null, Media.ChangeHeight ? (uint?)Media.NewHeight : null),
                Configuration.EditsPath,
                Media.NewFileName,
                Media.NewFormat);

            if (!Directory.Exists(Configuration.EditsPath)) { Directory.CreateDirectory(Configuration.EditsPath); }

            process.Start();
            process.OutputDataReceived += OnLogReceived;
            process.ErrorDataReceived += OnLogReceived;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();
            Monitor.Exit(editLock);
        }
        catch (Exception e)
        {
            return new(EditError.Other, e.Message);
        }
        return null;
    }

    public async Task<Error<MetadataError>?> MetadataAsync()
    {
        if (!File.Exists(Configuration.FFprobePath))
        {
            return new(MetadataError.NoFFProbe, Resources.NoFFProbe_Metadata_Error);
        }
        if (!File.Exists(Media.Path))
        {
            return new(MetadataError.NoFile, Resources.NoFile_Metadata_Error);
        }

        try
        {
            var process = FFmpeg.Metadata.Process(
                Configuration.FFprobePath,
                Media.Path,
                [FFmpeg.Metadata.StreamEntry.Width, FFmpeg.Metadata.StreamEntry.Height],
                [FFmpeg.Metadata.FormatEntry.Duration]);
            process.Start();
            process.BeginErrorReadLine();
            process.ErrorDataReceived += OnLogReceived;

            var output = await process.StandardOutput.ReadToEndAsync();
            var metadata = JsonSerializer.Deserialize<FFmpeg.Metadata.Output>(output, serializerOptions);

            var duration = metadata.Format.Duration.Split(':');
            Media.Duration = new MediaTime()
            {
                Hours = uint.Parse(duration[0]),
                Minutes = uint.Parse(duration[1]),
                Seconds = (uint)float.Parse(duration[2])
            };

            if (metadata.Streams.Any())
            {
                Media.Width = metadata.Streams.First().Width;
                Media.Height = metadata.Streams.First().Height;
            }
            else
            {
                Media.Width = null;
                Media.Height = null;
            }

            await process.WaitForExitAsync();
        }
        catch (Exception e)
        {
            return new(MetadataError.Other, e.Message);
        }

        return null;
    }

    private readonly object editLock = new();
    private readonly JsonSerializerOptions serializerOptions = new() { PropertyNameCaseInsensitive = true };

    private void OnLogReceived(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.Data)) { return; }
        Dispatcher.UIThread.InvokeAsync(() => Logger.AppendLine(e.Data));
    }
}
