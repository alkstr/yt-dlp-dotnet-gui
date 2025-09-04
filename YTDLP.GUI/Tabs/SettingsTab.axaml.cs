using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using YTDLP.GUI.ViewModels;

namespace YTDLP.GUI.Tabs;

public partial class SettingsTab : Tab<SettingsViewModel>
{
    public SettingsTab() : base(new SettingsViewModel()) => InitializeComponent();

    public async void ChangeDownloadFolderAsync(object sender, RoutedEventArgs args) =>
        await ChangePathAsync(nameof(ViewModel.DownloadPath), StorageItem.Folder);

    public async void ChangeEditsFolderAsync(object sender, RoutedEventArgs args) =>
        await ChangePathAsync(nameof(ViewModel.EditsPath), StorageItem.Folder);

    public void OpenAppFolder(object sender, RoutedEventArgs args) =>
        OpenPath(Directory.GetCurrentDirectory());

    public void OpenAppRepositoryURL(object sender, RoutedEventArgs args) =>
        OpenPath("https://github.com/alkstr/yt-dlp-gui");

    private enum StorageItem
    {
        File,
        Folder
    }

    private async Task ChangePathAsync(string pathPropertyName, StorageItem storageItemType)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
        {
            return;
        }

        IReadOnlyList<IStorageItem> storageItems = storageItemType switch
        {
            StorageItem.File => await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false
            }),
            StorageItem.Folder => await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                AllowMultiple = false
            }),
            _ => []
        };

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
        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
    }
}