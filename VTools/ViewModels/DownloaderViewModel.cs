using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using VTools.Models;

namespace VTools.ViewModels;

public partial class DownloaderViewModel : ViewModelBase
{
    public Media Media { get; } = new();
    public ObservableCollection<string> Logs { get; } = [];

    public async Task<YTDLP.DownloadResult> DownloadAsync()
    {
        if (Monitor.IsEntered(DownloadLock))
        {
            return YTDLP.DownloadResult.AnotherInProgressError;
        }

        Monitor.Enter(DownloadLock);
        Logs.Clear();
        var result = await YTDLP.DownloadAsync(Media, OnLogReceived);

        Monitor.Exit(DownloadLock);
        return result;
    }

    private readonly object DownloadLock = new();

    private void OnLogReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.Data))
        {
            Dispatcher.UIThread.InvokeAsync(() => Logs.Add(e.Data));
        }
    }
}
