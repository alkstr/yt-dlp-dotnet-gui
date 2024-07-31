using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using VTools.Utilities;

namespace VTools.Tabs;

public partial class SettingsTab : UserControl
{
    public SettingsTab()
    {
        InitializeComponent();
    }

    public async void ChangeDownloadDirectoryAsync(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this)!;
        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            AllowMultiple = false
        });

        if (folders != null && folders.Count > 0 && !string.IsNullOrWhiteSpace(folders[0].Path.ToString()))
        {
            Configuration.DownloadDirectory = folders[0].TryGetLocalPath()!;
            Configuration.SaveToFile();
            Configuration.Apply();
        }
    }

    internal void OpenYTDLPLink(object sender, RoutedEventArgs args)
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = "https://github.com/yt-dlp/yt-dlp/releases/",
            UseShellExecute = true
        });
    }

    internal void OpenFFMPEGLink(object sender, RoutedEventArgs args)
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = "https://github.com/BtbN/FFmpeg-Builds/releases",
            UseShellExecute = true
        });
    }

    public void OpenAppDirectory(object sender, RoutedEventArgs args)
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = Directory.GetCurrentDirectory(),
            UseShellExecute = true
        });
    }
}