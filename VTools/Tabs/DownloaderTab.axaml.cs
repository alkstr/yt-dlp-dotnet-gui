using Avalonia.Controls;
using Avalonia.Interactivity;
using VTools.ViewModels;
using VTools.Views;

namespace VTools.Tabs;

public partial class DownloaderTab : Tab<DownloaderViewModel>
{
    public DownloaderTab() : base(new DownloaderViewModel()) => InitializeComponent();

    private async void DownloadAsync(object sender, RoutedEventArgs args)
    {
        var result = await ViewModel.DownloadAsync();
        switch (result)
        {
            case DownloaderViewModel.DownloadResult.AlreadyDownloading:
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, "Another download in progress");
                return;
            case DownloaderViewModel.DownloadResult.ExecutableNotFound:
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, "yt-dlp executable is not found");
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
            case DownloaderViewModel.ChangeMetadataResult.Canceled:
                return;
            case DownloaderViewModel.ChangeMetadataResult.EmptyURLError:
                return;
            case DownloaderViewModel.ChangeMetadataResult.YTDLPNotFoundError:
                ViewUtilities.ShowAttachedFlyoutWithText(ThumbnailImage, $"yt-dlp executable is not found");
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
        TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(string.Join('\n', ViewModel.Logger.Lines));

    private void ClearLogs(object sender, RoutedEventArgs args) => ViewModel.Logger.Clear();
}
