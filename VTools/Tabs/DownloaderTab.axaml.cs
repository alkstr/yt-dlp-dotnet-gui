using System;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using VTools.ViewModels;
using VTools.Views;

namespace VTools.Tabs;

public partial class DownloaderTab : Tab<DownloaderViewModel>
{
    public DownloaderTab() : base(new DownloaderViewModel())
    {
        InitializeComponent();
        ViewModel.Logs.CollectionChanged += OnLogsChanged;
    }

    private async void DownloadAsync(object? sender, RoutedEventArgs args)
    {
        if (ViewModel is null || sender is null)
        {
            throw new NullReferenceException();
        }

        var result = await ViewModel.DownloadAsync();
        switch (result)
        {
            case YTDLP.DownloadResult.Success:
            {
                return;
            }
            case YTDLP.DownloadResult.InvalidInput:
            {
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, "Invalid input");
                return;
            }
            case YTDLP.DownloadResult.AnotherInProgressError:
            {
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, "Another download in progress");
                return;
            }
            case YTDLP.DownloadResult.ExecutableNotFoundError:
            {
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, "yt-dlp.exe is not found");
                return;
            }
        }
    }

    private void OnLogsChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        Dispatcher.UIThread.InvokeAsync(() => LogsListBox.ScrollIntoView(LogsListBox.ItemCount - 1));
}
