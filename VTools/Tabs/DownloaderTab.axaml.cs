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
        var error = await ViewModel.DownloadAsync();
        if (error != null) { ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, error.Message); }
    }

    private async void OnURLChanged(object sender, TextChangedEventArgs e)
    {
        var error = await ViewModel.ChangeMetadataAsync();
        if (error != null) { ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, error.Message); }
    }
}
