using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using VTools.Models;
using VTools.Utilities;

namespace VTools.ViewModels;

public partial class DownloaderViewModel : ViewModelBase
{
    public WebMediaFile Media { get; } = new();
    public Logger Logger { get; } = new();
    public byte MetadataLoadersCount
    {
        get => metadataLoadersCount;
        private set => SetProperty(ref metadataLoadersCount, value);
    }

    public enum DownloadResult
    {
        Finished,
        AlreadyDownloading,
        ExecutableNotFound,
    }

    public enum ChangeMetadataResult
    {
        Finished,
        Canceled,
        EmptyURLError,
        YTDLPNotFoundError,
        InvalidOutputError,
        ThumbnailFetchError,
    }

    public async Task<DownloadResult> DownloadAsync()
    {
        if (Monitor.IsEntered(DownloadLock))
        {
            return DownloadResult.AlreadyDownloading;
        }
        if (!File.Exists(Configuration.YTDLPPath))
        {
            return DownloadResult.ExecutableNotFound;
        }

        Monitor.Enter(DownloadLock);
        Logger.Clear();

        var process = YTDLP.DownloadProcess(
            Configuration.YTDLPPath,
            Configuration.FFmpegPath,
            Media.URL,
            Media.Format.Type,
            Media.Subtitles.Type,
            Configuration.DownloadPath
        );
        process.Start();
        process.OutputDataReceived += OnLogReceived;
        process.ErrorDataReceived += OnLogReceived;
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();
        Monitor.Exit(DownloadLock);

        return DownloadResult.Finished;
    }

    public async Task<ChangeMetadataResult> ChangeMetadataAsync()
    {
        if (string.IsNullOrWhiteSpace(Media.URL))
        {
            return ChangeMetadataResult.EmptyURLError;
        }
        if (!File.Exists(Configuration.YTDLPPath))
        {
            return ChangeMetadataResult.YTDLPNotFoundError;
        }

        try
        {
            MetadataLoadersCount++;

            await cancellationTokenSource.CancelAsync();
            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            Media.Title = "";
            Media.Channel = "";
            Media.Thumbnail = null;

            var metadataFields = new[] { "thumbnail", "title", "channel" };
            var process = YTDLP.GetMetadataProcess(new YTDLP.MetadataInfo
            {
                ExecutablePath = Configuration.YTDLPPath,
                URL = Media.URL,
                Fields = metadataFields
            });

            // Starting a new process for each key press would be too resource-intensive.
            // Instead, let's wait briefly to ensure the user has typed the entire URL.
            await Task.Delay(3000).WaitAsync(cancellationToken);
            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            Logger.AppendLine(await process.StandardError.ReadToEndAsync(cancellationToken));

            var lines = output.Split('\n');
            if (lines.Length < metadataFields.Length) { throw new InvalidDataException(); }
            var thumbnailURL = lines[0];
            Media.Title = lines[1];
            Media.Channel = lines[2];

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(thumbnailURL, cancellationToken);
            response.EnsureSuccessStatusCode();
            var thumbnailBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

            if (thumbnailBytes.Length > 0)
            {
                Media.Thumbnail = new Bitmap(new MemoryStream(thumbnailBytes));
            }

            await process.WaitForExitAsync();
            return ChangeMetadataResult.Finished;
        }
        catch (OperationCanceledException)
        {
            return ChangeMetadataResult.Canceled;
        }
        catch (HttpRequestException)
        {
            return ChangeMetadataResult.ThumbnailFetchError;
        }
        catch (InvalidDataException)
        {
            return ChangeMetadataResult.InvalidOutputError;
        }
        finally
        {
            MetadataLoadersCount--;
        }
    }

    private readonly object DownloadLock = new();
    private CancellationTokenSource cancellationTokenSource = new();
    private byte metadataLoadersCount;

    private void OnLogReceived(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.Data)) { return; }
        Dispatcher.UIThread.InvokeAsync(() => Logger.AppendLine(e.Data));
    }
}