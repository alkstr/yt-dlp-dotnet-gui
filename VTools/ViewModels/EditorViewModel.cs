using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using VTools.Models;
using VTools.Utilities;

namespace VTools.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    public LocalMediaFile Media { get; } = new();
    public Logger Logger { get; } = new();

    public enum EditResult
    {
        Success,
        NoFFmpegError,
        NoFileError,
        AnotherInProgressError,
    }

    public enum DurationResult
    {
        Success,
        NoFFprobeError,
        NoFileError,
        InvalidOutputError,
    }

    public async Task<EditResult> EditAsync()
    {
        if (!File.Exists(Configuration.FFmpegPath)) { return EditResult.NoFFmpegError; }
        if (!File.Exists(Media.Path)) { return EditResult.NoFileError; }
        if (Monitor.IsEntered(editLock)) { return EditResult.AnotherInProgressError; }

        Monitor.Enter(editLock);
        Logger.Clear();

        var process = FFmpeg.EditProcess(
            Configuration.FFmpegPath,
            Media.Path,
            Media.WillBeCut ? (Media.CutStart.ToString(), Media.CutEnd.ToString()) : (null, null),
            Media.EditedFileName,
            Media.Format);
        process.Start();
        process.OutputDataReceived += OnLogReceived;
        process.ErrorDataReceived += OnLogReceived;
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();
        Monitor.Exit(editLock);
        return EditResult.Success;
    }

    public async Task<DurationResult> DurationAsync()
    {
        if (!File.Exists(Configuration.FFprobePath)) { return DurationResult.NoFFprobeError; }
        if (!File.Exists(Media.Path)) { return DurationResult.NoFileError; }

        var process = FFmpeg.DurationProcess(Configuration.FFprobePath, Media.Path);
        process.Start();
        process.BeginErrorReadLine();
        process.ErrorDataReceived += OnLogReceived;

        var output = await process.StandardOutput.ReadToEndAsync();
        var duration = output.Split(':');
        if (duration.Length != 3) { return DurationResult.InvalidOutputError; }
        Media.Duration = new MediaTime()
        {
            Hours = uint.Parse(duration[0]),
            Minutes = uint.Parse(duration[1]),
            Seconds = (uint)float.Parse(duration[2])
        };
        Media.CutStart = new MediaTime();
        Media.CutEnd = Media.Duration;

        await process.WaitForExitAsync();
        return DurationResult.Success;
    }

    private readonly object editLock = new();

    private void OnLogReceived(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.Data)) { return; }
        Dispatcher.UIThread.InvokeAsync(() => Logger.AppendLine(e.Data));
    }
}
