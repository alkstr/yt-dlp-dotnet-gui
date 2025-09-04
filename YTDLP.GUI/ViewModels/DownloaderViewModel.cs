using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using YTDLP.GUI.Assets;
using YTDLP.GUI.Models;
using YTDLP.GUI.Utilities;

namespace YTDLP.GUI.ViewModels;

public class DownloaderViewModel : ViewModelBase
{
    public WebMediaFile Media { get; } = new();
    public Logger Logger { get; } = new();

    public byte DownloaderCount
    {
        get => downloaderCount;
        private set => SetProperty(ref downloaderCount, value);
    }

    public byte MetadataLoadersCount
    {
        get => metadataLoadersCount;
        private set => SetProperty(ref metadataLoadersCount, value);
    }

    public enum DownloadError
    {
        NoYTDLP,
        Other
    }

    public enum ChangeMetadataError
    {
        NoYTDLP,
        InvalidOutput,
        FailedToFetch,
        Other
    }

    public async Task<Error<DownloadError>?> DownloadOrCancelAsync()
    {
        if (!File.Exists(Configuration.YTDLPPath))
        {
            return new Error<DownloadError>(DownloadError.NoYTDLP, Resources.NoYTDLP_Download_Error);
        }

        if (DownloaderCount > 0)
        {
            await downloaderCancellationTokenSource.CancelAsync();
            foreach (var process in Process.GetProcessesByName("yt-dlp"))
            {
                process.Kill();
            }

            return null;
        }

        try
        {
            DownloaderCount++;

            downloaderCancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = downloaderCancellationTokenSource.Token;

            downloader = YouTube.Download.Process(
                Configuration.YTDLPPath,
                Configuration.FFmpegPath,
                Media.URL,
                Media.Format.Type,
                Media.Subtitles.Type,
                Configuration.IsProxyEnabled.IsFalse() ? null : Configuration.Proxy,
                Configuration.DownloadPath);

            downloader.Start();
            downloader.OutputDataReceived += OnLogReceived;
            downloader.ErrorDataReceived += OnLogReceived;
            downloader.BeginOutputReadLine();
            downloader.BeginErrorReadLine();
            await downloader.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception e)
        {
            return new Error<DownloadError>(DownloadError.Other, e.Message);
        }
        finally
        {
            downloader = null;
            DownloaderCount--;
        }

        return null;
    }

    public async Task<Error<ChangeMetadataError>?> ChangeMetadataAsync()
    {
        if (string.IsNullOrWhiteSpace(Media.URL))
        {
            return null;
        }

        if (!File.Exists(Configuration.YTDLPPath))
        {
            return new Error<ChangeMetadataError>(ChangeMetadataError.NoYTDLP, Resources.NoYTDLP_ChangeMetadata_Error);
        }

        try
        {
            MetadataLoadersCount++;

            await metadataLoaderCancellationTokenSource.CancelAsync();
            metadataLoaderCancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = metadataLoaderCancellationTokenSource.Token;

            Media.Title = "";
            Media.Channel = "";
            Media.Thumbnail = null;

            var metadataFields = new[]
            {
                YouTube.Metadata.Field.ThumbnailURL,
                YouTube.Metadata.Field.Title,
                YouTube.Metadata.Field.Channel
            };
            var process = YouTube.Metadata.Process(
                Configuration.YTDLPPath,
                Media.URL,
                Configuration.IsProxyEnabled.IsFalse() ? null : Configuration.Proxy,
                metadataFields);

            // Starting a new process for each key press would be too resource-intensive.
            // Instead, let's wait briefly to ensure the user has typed the entire URL.
            await Task.Delay(3000, cancellationToken).WaitAsync(cancellationToken);
            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            Logger.AppendLine(await process.StandardError.ReadToEndAsync(cancellationToken));

            var lines = output.Split('\n');
            if (lines.Length < metadataFields.Length)
            {
                throw new InvalidDataException();
            }

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

            await process.WaitForExitAsync(cancellationToken);
            return null;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (HttpRequestException)
        {
            return new Error<ChangeMetadataError>(ChangeMetadataError.FailedToFetch,
                Resources.FailedToFetch_ChangeMetadata_Error);
        }
        catch (InvalidDataException)
        {
            return new Error<ChangeMetadataError>(ChangeMetadataError.InvalidOutput,
                Resources.InvalidOutput_ChangeMetadata_Error);
        }
        catch (Exception e)
        {
            return new Error<ChangeMetadataError>(ChangeMetadataError.Other, e.Message);
        }
        finally
        {
            MetadataLoadersCount--;
        }
    }

    private CancellationTokenSource downloaderCancellationTokenSource = new();
    private byte downloaderCount;
    private Process? downloader;

    private CancellationTokenSource metadataLoaderCancellationTokenSource = new();
    private byte metadataLoadersCount;

    private void OnLogReceived(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.Data))
        {
            return;
        }

        Dispatcher.UIThread.InvokeAsync(() => Logger.AppendLine(e.Data));
    }
}