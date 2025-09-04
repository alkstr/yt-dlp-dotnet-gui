using Avalonia.Controls;
using Avalonia.Interactivity;
using YTDLP.GUI.ViewModels;
using YTDLP.GUI.Views;

namespace YTDLP.GUI.Tabs;

public partial class DownloaderTab : Tab<DownloaderViewModel>
{
    public DownloaderTab() : base(new DownloaderViewModel()) => InitializeComponent();

    private async void DownloadOrCancelAsync(object sender, RoutedEventArgs args)
    {
        var error = await ViewModel.DownloadOrCancelAsync();
        if (error != null)
        {
            ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, error.Message);
        }
    }

    private async void RefreshAsync(object sender, RoutedEventArgs args)
    {
        var error = await ViewModel.ChangeMetadataAsync();
        if (error != null)
        {
            ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, error.Message);
        }
    }

    private async void OnURLChanged(object sender, TextChangedEventArgs e)
    {
        var error = await ViewModel.ChangeMetadataAsync();
        if (error != null)
        {
            ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, error.Message);
        }
    }
}