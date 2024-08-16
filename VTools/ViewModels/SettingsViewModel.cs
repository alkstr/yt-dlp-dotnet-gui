using System.Globalization;
using VTools.Utilities;

namespace VTools.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public CultureInfo[] AvailableCultures { get; } = Configuration.AvailableCultures;

        public CultureInfo CurrentCulture
        {
            get => Configuration.Culture;
            set
            {
                Configuration.Culture = value;
                Configuration.Apply();
                OnPropertyChanged(nameof(CurrentCulture));
                Configuration.SaveToFile();
            }
        }

        public string DownloadDirectory
        {
            get => Configuration.DownloadDirectory;
            set
            {
                Configuration.DownloadDirectory = value;
                Configuration.Apply();
                OnPropertyChanged(nameof(CurrentCulture));
                Configuration.SaveToFile();
            }
        }

        public string YTDLPPath
        {
            get => Configuration.YTDLPPath;
            set
            {
                Configuration.YTDLPPath = value;
                Configuration.Apply();
                OnPropertyChanged(nameof(YTDLPPath));
                Configuration.SaveToFile();
            }
        }

        public string FFmpegPath
        {
            get => Configuration.FFmpegPath;
            set
            {
                Configuration.FFmpegPath = value;
                Configuration.Apply();
                OnPropertyChanged(nameof(FFmpegPath));
                Configuration.SaveToFile();
            }
        }
    }
}
