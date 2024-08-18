using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using VTools.ViewModels;

namespace VTools.Tabs;

public partial class SettingsTab : Tab<SettingsViewModel>
{
    public SettingsTab() : base(new SettingsViewModel())
    {
        InitializeComponent();
    }

    public async void ChangeDownloadPathAsync(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this)!;
        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            AllowMultiple = false
        });

        if (folders != null && folders.Count > 0 && !string.IsNullOrWhiteSpace(folders[0].Path.ToString()))
        {
            ViewModel.DownloadPath = folders[0].TryGetLocalPath()!;
        }
    }

    public async void ChangeYTDLPPathAsync(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this)!;
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = false,
        });

        if (files != null && files.Count > 0 && !string.IsNullOrWhiteSpace(files[0].Path.ToString()))
        {
            ViewModel.YTDLPPath = files[0].TryGetLocalPath()!;
        }
    }

    public async void ChangeFFmpegPathAsync(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this)!;
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = false,
        });

        if (files != null && files.Count > 0 && !string.IsNullOrWhiteSpace(files[0].Path.ToString()))
        {
            ViewModel.FFmpegPath = files[0].TryGetLocalPath()!;
        }
    }

    public void OpenYTDLPRepositoryURL(object sender, RoutedEventArgs args) =>
        OpenPath("https://github.com/yt-dlp/yt-dlp/releases/");

    public void OpenFFmpegRepositoryURL(object sender, RoutedEventArgs args) =>
        OpenPath("https://github.com/BtbN/FFmpeg-Builds/releases");

    public void OpenAppDirectory(object sender, RoutedEventArgs args) =>
        OpenPath(Directory.GetCurrentDirectory());

    public void OpenRepositoryURL(object sender, RoutedEventArgs args) =>
        OpenPath("https://github.com/alkstr/VTools");

    private static void OpenPath(string path)
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = path,
            UseShellExecute = true
        });
    }
}