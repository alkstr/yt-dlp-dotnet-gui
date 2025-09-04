using YTDLP.Dotnet.GUI.Utilities;

namespace YTDLP.Dotnet.GUI.ViewModels;

public class DesignDownloaderViewModel : DownloaderViewModel
{
    public DesignDownloaderViewModel() => Configuration.LoadFromFileOrDefault();
}

public class DesignEditorViewModel : EditorViewModel
{
    public DesignEditorViewModel() => Configuration.LoadFromFileOrDefault();
}

public class DesignSettingsViewModel : SettingsViewModel
{
    public DesignSettingsViewModel() => Configuration.LoadFromFileOrDefault();
}