using System.Globalization;
using VTools.Utilities;

namespace VTools.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public static CultureInfo[] AvailableCultures { get; } = Configuration.AvailableCultures;

    public static CultureInfo CurrentCulture
    {
        get => Configuration.Culture;
        set
        {
            Configuration.Culture = value;
            Configuration.Apply();
            Configuration.SaveToFile();
        }
    }

    public static string DownloadDirectory
    {
        get => Configuration.DownloadDirectory;
        set
        {
            Configuration.DownloadDirectory = value;
            Configuration.Apply();
            Configuration.SaveToFile();
        }
    }
}
