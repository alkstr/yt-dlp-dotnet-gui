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
    public WebMedia Media { get; } = new();
    public Logger Logger { get; } = new();

    public enum DownloadResult
    {
        Finished,
        AlreadyDownloading,
        ExecutableNotFound,
    }

    public enum ChangeMetadataResult
    {
        Finished,
        EmptyURLError,
        ExecutableNotFoundError,
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

        var process = YTDLP.GetDownloadProcess(new YTDLP.DownloadInfo
        {
            ExecutablePath = Configuration.YTDLPPath,
            URL = Media.URL,
            Format = Media.Format,
            Directory = Configuration.DownloadDirectory
        });
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
            return ChangeMetadataResult.ExecutableNotFoundError;
        }

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

        string output;
        try
        {
            await Task.Delay(3000).WaitAsync(cancellationToken);
            process.Start();

            using var reader = process.StandardOutput;
            output = await reader.ReadToEndAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return ChangeMetadataResult.Finished;
        }

        var lines = output.Split('\n');
        if (lines.Length < metadataFields.Length)
        {
            return ChangeMetadataResult.InvalidOutputError;
        }

        var thumbnailURL = lines[0];
        Media.Title = lines[1];
        Media.Channel = lines[2];

        using var httpClient = new HttpClient();
        byte[] thumbnailBytes;
        try
        {
            var response = await httpClient.GetAsync(thumbnailURL, cancellationToken);
            response.EnsureSuccessStatusCode();
            thumbnailBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }
        catch (HttpRequestException)
        {
            return ChangeMetadataResult.ThumbnailFetchError;
        }
        catch (OperationCanceledException)
        {
            return ChangeMetadataResult.Finished;
        }
        if (thumbnailBytes.Length > 0)
        {
            Media.Thumbnail = new Bitmap(new MemoryStream(thumbnailBytes));
        }

        await process.WaitForExitAsync();
        return ChangeMetadataResult.Finished;
    }

    private readonly object DownloadLock = new();
    private CancellationTokenSource cancellationTokenSource = new();

    private void OnLogReceived(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.Data)) { return; }
        Dispatcher.UIThread.InvokeAsync(() => Logger.AppendLine(e.Data));
    }
}
