using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace VTools.Tabs;

public partial class SettingsTab : UserControl
{
    public SettingsTab()
    {
        InitializeComponent();
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