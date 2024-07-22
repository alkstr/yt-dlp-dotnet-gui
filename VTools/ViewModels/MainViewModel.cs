using System.Globalization;
using VTools.Utilities;

namespace VTools.ViewModels;

public partial class MainViewModel(Configuration config) : ViewModelBase
{
    public CultureInfo CurrentCulture
    {
        get => config.Culture;
        set
        {
            config.Culture = value;
            config.Apply();
            config.Save();
        }
    }

    private readonly Configuration config = config;
}
