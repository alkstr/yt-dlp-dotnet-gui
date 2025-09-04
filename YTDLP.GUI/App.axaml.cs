using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using YTDLP.GUI.ViewModels;
using YTDLP.GUI.Views;
using YTDLP.GUI.Utilities;

namespace YTDLP.GUI;

public class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);
        Configuration.LoadFromFileOrDefault();
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindow { DataContext = new MainViewModel() };
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainWindow { DataContext = new MainViewModel() };
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}