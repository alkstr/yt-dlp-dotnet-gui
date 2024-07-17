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

    private async void DownloadAsync(object sender, RoutedEventArgs args)
    {
        var result = await ViewModel.DownloadAsync();
        switch (result)
        {
            case DownloaderViewModel.DownloadResult.AlreadyDownloading:
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, "Another download in progress");
                return;
            case DownloaderViewModel.DownloadResult.ExecutableNotFound:
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, $"{YTDLP.ExecutableName} is not found");
                return;
            case DownloaderViewModel.DownloadResult.Finished:
                return;
        }
    }

    private async void OnURLChanged(object? sender, TextChangedEventArgs e)
    {
        if (ViewModel is null || sender is null)
        {
            throw new NullReferenceException();
        }

        await ViewModel.ChangeMetadataAsync();
    }

    private void CopyLogs(object sender, RoutedEventArgs args) =>
        TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(string.Join('\n', ViewModel.Logs));

    private void ClearLogs(object sender, RoutedEventArgs args) => ViewModel.Logs.Clear();

    private void OnLogsChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        Dispatcher.UIThread.InvokeAsync(() => LogsListBox.ScrollIntoView(LogsListBox.ItemCount - 1));
}
