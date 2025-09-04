using YTDLP.Dotnet.GUI.Utilities;

namespace YTDLP.Dotnet.GUI.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    public static string[] AvailableCultureNames => Configuration.AvailableCultureNames;

    public string CultureName
    {
        get => Configuration.CultureName;
        set
        {
            Configuration.Set(nameof(Configuration.CultureName), value);
            OnPropertyChanged();
        }
    }

    public string DownloadPath
    {
        get => Configuration.DownloadPath;
        set
        {
            Configuration.Set(nameof(Configuration.DownloadPath), value);
            OnPropertyChanged();
        }
    }

    public string EditsPath
    {
        get => Configuration.EditsPath;
        set
        {
            Configuration.Set(nameof(Configuration.EditsPath), value);
            OnPropertyChanged();
        }
    }

    public string YTDLPPath
    {
        get => Configuration.YTDLPPath;
        set
        {
            Configuration.Set(nameof(Configuration.YTDLPPath), value);
            OnPropertyChanged();
        }
    }

    public string FFmpegPath
    {
        get => Configuration.FFmpegPath;
        set
        {
            Configuration.Set(nameof(Configuration.FFmpegPath), value);
            OnPropertyChanged();
        }
    }

    public string FFprobePath
    {
        get => Configuration.FFprobePath;
        set
        {
            Configuration.Set(nameof(Configuration.FFprobePath), value);
            OnPropertyChanged();
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
            OnPropertyChanged();
        }
    }

    public void UpdateYTDLP() => YouTube.Update.Process(Configuration.YTDLPPath).Start();
}