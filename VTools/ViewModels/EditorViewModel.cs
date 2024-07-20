using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using VTools.Models;
using VTools.Utilities;

namespace VTools.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    public MediaForEdit Media { get; } = new();
    public Logger Logger { get; } = new();

    public async Task<FFMPEG.EditResult> EditAsync()
    {
        if (Monitor.IsEntered(editLock))
        {
            return FFMPEG.EditResult.AnotherInProgressError;
        }

        Monitor.Enter(editLock);

        Logger.Clear();
        var result = await FFMPEG.EditAsync(Media, OnLogReceived);

        Monitor.Exit(editLock);
        return result;
    }

    public async void GetMediaDurationAsync()
    {
        Media.Duration = await FFMPEG.GetMediaDurationAsync(Media) ?? new MediaTime();
        Media.CutStart = new MediaTime();
        Media.CutEnd = Media.Duration;
    }

    private readonly object editLock = new();

    private void OnLogReceived(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.Data)) { return; }
        Dispatcher.UIThread.InvokeAsync(() => Logger.AppendLine(e.Data));
    }
}
