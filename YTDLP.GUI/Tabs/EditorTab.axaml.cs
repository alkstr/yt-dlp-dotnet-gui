using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using YTDLP.GUI.ViewModels;
using YTDLP.GUI.Views;

namespace YTDLP.GUI.Tabs;

public partial class EditorTab : Tab<EditorViewModel>
{
    public EditorTab() : base(new EditorViewModel()) => InitializeComponent();

    private async void ChooseFileAsync(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Open Media File",
                AllowMultiple = false
            });

        if (!files.Any())
        {
            return;
        }

        var path = files[0].TryGetLocalPath()!;
        ViewModel.Media.Path = path;
        MediaPathTextBox.CaretIndex = path.Length;
    }

    private async void EditAsync(object sender, RoutedEventArgs args)
    {
        var error = await ViewModel.EditAsync();
        if (error != null)
        {
            ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, error.Message);
        }
    }

    private async void MetadataAsync(object sender, RoutedEventArgs args)
    {
        var error = await ViewModel.MetadataAsync();
        if (error != null)
        {
            ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, error.Message);
        }
    }
}