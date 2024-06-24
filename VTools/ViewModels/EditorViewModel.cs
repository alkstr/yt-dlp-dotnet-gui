using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using VTools.Models;

namespace VTools.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    public MediaForEdit Media { get; } = new();
    public ObservableCollection<string> Logs { get; } = [];

    public async Task<FFMPEG.EditResult> EditAsync()
    {
        if (Monitor.IsEntered(editLock))
        {
            return FFMPEG.EditResult.AnotherInProgressError;
        }

        Monitor.Enter(editLock);

        Logs.Clear();
        var result = await FFMPEG.EditAsync(Media, OnLogReceived);

        Monitor.Exit(editLock);
        return result;
    }

    private readonly object editLock = new();

    private void OnLogReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.Data))
        {
            Dispatcher.UIThread.InvokeAsync(() => Logs.Add(e.Data));
        }
    }
}
