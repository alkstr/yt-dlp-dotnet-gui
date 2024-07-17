using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using VTools.Models;

namespace VTools.ViewModels;

public partial class DownloaderViewModel : ViewModelBase
{
    public WebMedia Media { get; } = new();
    public ObservableCollection<string> Logs { get; } = [];

    public enum DownloadResult
    {
        Finished,
        AlreadyDownloading,
        ExecutableNotFound,
    }

    public async Task<DownloadResult> DownloadAsync()
    {
        if (Monitor.IsEntered(DownloadLock))
        {
            return DownloadResult.AlreadyDownloading;
        }
        if (!File.Exists(YTDLP.ExecutableName))
        {
            return DownloadResult.ExecutableNotFound;
        }

        Monitor.Enter(DownloadLock);
        Logs.Clear();

        var process = YTDLP.GetDownloadProcess(new YTDLP.DownloadInfo { URL = Media.URL, Format = Media.Format });
        process.Start();
        process.OutputDataReceived += OnLogReceived;
        process.ErrorDataReceived += OnLogReceived;
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();
        Monitor.Exit(DownloadLock);

        return DownloadResult.Finished;
    }

    public async Task ChangeMetadataAsync()
    {
        if (string.IsNullOrWhiteSpace(Media.URL))
        {
            return;
        }

        await cancellationTokenSource.CancelAsync();
        cancellationTokenSource = new CancellationTokenSource();

        try
        {
            var metadata = await YTDLP.MetadataAsync(Media, cancellationTokenSource.Token);
            Media.Title = metadata.Title;
            Media.Channel = metadata.Channel;
            if (metadata.Thumbnail != null && metadata.Thumbnail.Length > 0)
            {
                Media.Thumbnail = new Bitmap(new MemoryStream(metadata.Thumbnail));
            }
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    private readonly object DownloadLock = new();
    private CancellationTokenSource cancellationTokenSource = new();

    private void OnLogReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.Data))
        {
            Dispatcher.UIThread.InvokeAsync(() => Logs.Add(e.Data));
        }
    }
}
