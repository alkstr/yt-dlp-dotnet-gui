using VTools.Utilities;

namespace VTools.ViewModels
{
    public class DesignDownloaderViewModel : DownloaderViewModel
    {
        public DesignDownloaderViewModel()
        {
            Assets.Resources.Culture = Configuration.AvailableCultures[1];
        }
    }

    public class DesignEditorViewModel : EditorViewModel
    {
        public DesignEditorViewModel()
        {
            Assets.Resources.Culture = Configuration.AvailableCultures[1];
        }
    }

    public class DesignSettingsViewModel : SettingsViewModel
    {
        public DesignSettingsViewModel()
        {
            Assets.Resources.Culture = Configuration.AvailableCultures[1];
        }
    }
}