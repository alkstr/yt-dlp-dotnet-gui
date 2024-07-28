using System;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using VTools.ViewModels;
using VTools.Views;

namespace VTools.Tabs;

public partial class EditorTab : Tab<EditorViewModel>
{
    public EditorTab() : base(new EditorViewModel())
    {
        InitializeComponent();
        ViewModel.Logger.PropertyChanged += OnLogsChanged;
        logsTimer.Elapsed += (object? sender, ElapsedEventArgs e) =>
            Dispatcher.UIThread.InvokeAsync(() =>
                LogsTextBox.Text = ViewModel.Logger.ToString());
        logsTimer.Start();
    }

    private readonly Timer logsTimer = new(5000);

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

    private void OnLogsChanged(object? sender, PropertyChangedEventArgs e) =>
        Dispatcher.UIThread.InvokeAsync(() => LogsTextBox.CaretIndex = LogsTextBox.Text?.Length ?? 0);
}