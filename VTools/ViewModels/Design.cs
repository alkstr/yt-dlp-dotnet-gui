using VTools.Utilities;

namespace VTools.ViewModels
{
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
}