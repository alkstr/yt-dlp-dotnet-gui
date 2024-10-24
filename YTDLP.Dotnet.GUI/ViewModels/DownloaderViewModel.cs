using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using YTDLP.Dotnet.GUI.Assets;
using YTDLP.Dotnet.GUI.Models;
using YTDLP.Dotnet.GUI.Utilities;

namespace YTDLP.Dotnet.GUI.ViewModels;

public partial class DownloaderViewModel : ViewModelBase
{
    public WebMediaFile Media { get; } = new();
    public Logger Logger { get; } = new();
    public byte MetadataLoadersCount
    {
        get => metadataLoadersCount;
        private set => SetProperty(ref metadataLoadersCount, value);
    }

    public enum DownloadError
    {
        AnotherInProgress,
        NoYTDLP,
        Other,
    }

    public enum ChangeMetadataError
    {
        NoURL,
        NoYTDLP,
        InvalidOutput,
        FailedToFetch,
        Other,
    }

    public async Task<Error<DownloadError>?> DownloadAsync()
    {
        if (Monitor.IsEntered(DownloadLock))
        {
            return new(DownloadError.AnotherInProgress, Resources.AnotherInProgress_Download_Error);
        }
        if (!File.Exists(Configuration.YTDLPPath))
        {
            return new(DownloadError.NoYTDLP, Resources.NoYTDLP_Download_Error);
        }

        try
        {
            Monitor.Enter(DownloadLock);
            Logger.Clear();

            var process = YouTube.Download.Process(
                Configuration.YTDLPPath,
                Configuration.FFmpegPath,
                Media.URL,
                Media.Format.Type,
                Media.Subtitles.Type,
                Configuration.Proxy,
                string.IsNullOrWhiteSpace(Configuration.POToken) ? null : Configuration.POToken,
                string.IsNullOrWhiteSpace(Configuration.CookiesPath) ? null : Configuration.CookiesPath,
                Configuration.DownloadPath);
            process.Start();
            process.OutputDataReceived += OnLogReceived;
            process.ErrorDataReceived += OnLogReceived;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();
            Monitor.Exit(DownloadLock);
        }
        catch (Exception e)
        {
            return new(DownloadError.Other, e.Message);
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
            return new(ChangeMetadataError.NoYTDLP, Resources.NoYTDLP_ChangeMetadata_Error);
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

            var metadataFields = new[]
            {
                YouTube.Metadata.Field.ThumbnailURL,
                YouTube.Metadata.Field.Title,
                YouTube.Metadata.Field.Channel
            };
            var process = YouTube.Metadata.Process(
                Configuration.YTDLPPath,
                Media.URL,
                Configuration.Proxy,
                string.IsNullOrWhiteSpace(Configuration.POToken) ? null : Configuration.POToken,
                string.IsNullOrWhiteSpace(Configuration.CookiesPath) ? null : Configuration.CookiesPath,
                metadataFields);

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
            return null;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (HttpRequestException)
        {
            return new(ChangeMetadataError.FailedToFetch, Resources.FailedToFetch_ChangeMetadata_Error);
        }
        catch (InvalidDataException)
        {
            return new(ChangeMetadataError.InvalidOutput, Resources.InvalidOutput_ChangeMetadata_Error);
        }
        catch (Exception e)
        {
            return new(ChangeMetadataError.Other, e.Message);
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