using YTDLP.Dotnet.GUI.Utilities;

namespace YTDLP.Dotnet.GUI.ViewModels
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

        public string EditsPath
        {
            get => Configuration.EditsPath;
            set
            {
                Configuration.Set(nameof(Configuration.EditsPath), value);
                OnPropertyChanged(nameof(EditsPath));
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

    public bool IsProxyEnabled
    {
        get => Configuration.IsProxyEnabledAsBool;
        set
        {
            Configuration.Set(nameof(IsProxyEnabled), value ? "true" : "false");
            OnPropertyChanged();
        }
    }

        public string Proxy
        {
            get => Configuration.Proxy;
            set
            {
                Configuration.Set(nameof(Proxy), value);
                OnPropertyChanged(nameof(Proxy));
            }
        }

        public string POToken
        {
            get => Configuration.POToken;
            set
            {
                Configuration.Set(nameof(POToken), value);
                OnPropertyChanged(nameof(POToken));
            }
        }

        public string CookiesPath
        {
            get => Configuration.CookiesPath;
            set
            {
                Configuration.Set(nameof(CookiesPath), value);
                OnPropertyChanged(nameof(CookiesPath));
            }
        }

#pragma warning disable CA1822 // Mark members as static
        public void UpdateYTDLP() => YouTube.Update.Process(Configuration.YTDLPPath).Start();
#pragma warning restore CA1822 // Mark members as static
    }
}
