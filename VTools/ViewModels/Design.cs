using VTools.Utilities;

namespace VTools.ViewModels
{
    public class DesignMainViewModel : MainViewModel
    {
        public DesignMainViewModel()
        {
            Configuration.Culture = Configuration.AvailableCultures[1];
            Assets.Resources.Culture = Configuration.AvailableCultures[1];
        }
    }

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
}