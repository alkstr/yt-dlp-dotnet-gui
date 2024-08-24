using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using VTools.ViewModels;
using VTools.Views;

namespace VTools.Tabs;

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
                AllowMultiple = false,
            });
        if (files.Any())
        {
            var path = files[0].TryGetLocalPath()!;
            ViewModel.Media.Path = path;
            MediaPathTextBox.CaretIndex = path.Length;
            await ViewModel.DurationAsync();
        }
    }

    private async void EditAsync(object sender, RoutedEventArgs args)
    {
        if (ViewModel == null || sender == null) { throw new NullReferenceException(); }

        var result = await ViewModel.EditAsync();
        switch (result)
        {
            case EditorViewModel.EditResult.NoFFmpegError:
            {
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, "FFmpeg executable is not found");
                return;
            }
            case EditorViewModel.EditResult.NoFileError:
            {
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, "Editable file doesn't exist");
                return;
            }
            case EditorViewModel.EditResult.AnotherInProgressError:
            {
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, "Another edit in progress");
                return;
            }
            case EditorViewModel.EditResult.Success: { return; }
        }
    }
}