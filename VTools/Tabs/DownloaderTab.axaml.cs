using System.ComponentModel;
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
        ViewModel.Logger.PropertyChanged += OnLogsChanged;
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
        var result = await ViewModel.ChangeMetadataAsync();
        switch (result)
        {
            case DownloaderViewModel.ChangeMetadataResult.EmptyURLError:
                return;
            case DownloaderViewModel.ChangeMetadataResult.ExecutableNotFoundError:
                ViewUtilities.ShowAttachedFlyoutWithText(ThumbnailImage, $"{YTDLP.ExecutableName} is not found");
                return;
            case DownloaderViewModel.ChangeMetadataResult.InvalidOutputError:
                ViewUtilities.ShowAttachedFlyoutWithText(ThumbnailImage, $"yt-dlp returned the invalid metadata");
                return;
            case DownloaderViewModel.ChangeMetadataResult.ThumbnailFetchError:
                ViewUtilities.ShowAttachedFlyoutWithText(ThumbnailImage, $"Couldn't fetch the thumbnail");
                return;
            case DownloaderViewModel.ChangeMetadataResult.Finished:
                return;
        }
    }

    private void CopyLogs(object sender, RoutedEventArgs args) =>
        TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(ViewModel.Logger.String);

    private void ClearLogs(object sender, RoutedEventArgs args) => ViewModel.Logger.Clear();

    private void OnLogsChanged(object? sender, PropertyChangedEventArgs e) =>
        Dispatcher.UIThread.InvokeAsync(() => LogsTextBox.CaretIndex = LogsTextBox.Text?.Length ?? 0);
}
