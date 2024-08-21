using System.Globalization;
using VTools.Utilities;

namespace VTools.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public string[] AvailableCultureNames { get; } = Configuration.AvailableCultureNames;

        public string CultureName
        {
            get => Configuration.CultureName;
            set
            {
                Configuration.Set(nameof(Configuration.CultureName), value);
                OnPropertyChanged(nameof(CultureName));
            }
        }

        public string DownloadPath
        {
            get => Configuration.DownloadPath;
            set
            {
                Configuration.Set(nameof(Configuration.DownloadPath), value);
                OnPropertyChanged(nameof(DownloadPath));
            }
        }

        public string YTDLPPath
        {
            get => Configuration.YTDLPPath;
            set
            {
                Configuration.Set(nameof(Configuration.YTDLPPath), value);
                OnPropertyChanged(nameof(YTDLPPath));
            }
        }

        public string FFmpegPath
        {
            get => Configuration.FFmpegPath;
            set
            {
                Configuration.Set(nameof(Configuration.FFmpegPath), value);
                OnPropertyChanged(nameof(FFmpegPath));
            }
        }

        public string FFprobePath
        {
            get => Configuration.FFprobePath;
            set
            {
                Configuration.Set(nameof(Configuration.FFprobePath), value);
                OnPropertyChanged(nameof(FFprobePath));
            }
        }
    }
}
