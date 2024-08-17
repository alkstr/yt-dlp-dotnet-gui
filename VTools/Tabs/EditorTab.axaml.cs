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
            ViewModel.GetMediaDurationAsync();
        }
    }

    private async void EditAsync(object sender, RoutedEventArgs args)
    {
        if (ViewModel is null || sender is null)
        {
            throw new NullReferenceException();
        }

        var result = await ViewModel.EditAsync();
        switch (result)
        {
            case FFMPEG.EditResult.Success:
            {
                return;
            }
            case FFMPEG.EditResult.InvalidInput:
            {
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, "Invalid input");
                return;
            }
            case FFMPEG.EditResult.AnotherInProgressError:
            {
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, "Another edit in progress");
                return;
            }
            case FFMPEG.EditResult.ExecutableNotFoundError:
            {
                ViewUtilities.ShowAttachedFlyoutWithText((Control)sender, $"{FFMPEG.ExecutableName} is not found");
                return;
            }
        }
    }

    private void CopyLogs(object sender, RoutedEventArgs args) =>
        TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(ViewModel.Logger.ToString());

    private void ClearLogs(object sender, RoutedEventArgs args) => ViewModel.Logger.Clear();
}