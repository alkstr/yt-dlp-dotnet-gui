using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using VTools.Utilities;
using VTools.ViewModels;

namespace VTools.Tabs;

public partial class SettingsTab : Tab<SettingsViewModel>
{
    public SettingsTab() : base(new SettingsViewModel()) => InitializeComponent();

    public async void ChangeDownloadFolderAsync(object sender, RoutedEventArgs args) => 
        await ChangePathAsync(nameof(ViewModel.DownloadPath), StorageItem.Folder);

    public async void ChangeYTDLPPathAsync(object sender, RoutedEventArgs args) =>
        await ChangePathAsync(nameof(ViewModel.YTDLPPath), StorageItem.File);

    public async void ChangeFFmpegPathAsync(object sender, RoutedEventArgs args) =>
        await ChangePathAsync(nameof(ViewModel.FFmpegPath), StorageItem.File);

    public async void ChangeFFprobePathAsync(object sender, RoutedEventArgs args) =>
        await ChangePathAsync(nameof(ViewModel.FFprobePath), StorageItem.File);

    public void OpenYTDLPDownloadURL(object sender, RoutedEventArgs args) =>
        OpenPath("https://github.com/yt-dlp/yt-dlp/releases/");

    public void OpenFFmpegDownloadURL(object sender, RoutedEventArgs args) =>
        OpenPath("https://github.com/BtbN/FFmpeg-Builds/releases");

    public void OpenAppFolder(object sender, RoutedEventArgs args) =>
        OpenPath(Directory.GetCurrentDirectory());

    public void OpenAppRepositoryURL(object sender, RoutedEventArgs args) =>
        OpenPath("https://github.com/alkstr/VTools");

    private enum StorageItem
        {
        File,
        Folder,
        }

    private async Task ChangePathAsync(string pathPropertyName, StorageItem storageItemType)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) { return; }

        IReadOnlyList<IStorageItem> storageItems = [];
        switch (storageItemType)
        {
            case StorageItem.File:
                storageItems = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
    {
            AllowMultiple = false,
        });
                break;
            case StorageItem.Folder:
                storageItems = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            AllowMultiple = false,
        });
                break;
        }

        if (storageItems.Count > 0 && !string.IsNullOrWhiteSpace(storageItems[0].Path.ToString()))
        {
            ViewModel
                .GetType()
                .GetProperty(pathPropertyName)?
                .SetValue(ViewModel, storageItems[0].TryGetLocalPath()!);
        }
    }

    private static void OpenPath(string path)
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = path,
            UseShellExecute = true
        });
    }
}