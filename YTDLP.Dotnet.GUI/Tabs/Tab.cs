using Avalonia.Controls;
using YTDLP.Dotnet.GUI.ViewModels;

namespace YTDLP.Dotnet.GUI.Tabs;

public abstract class Tab<T> : UserControl where T : ViewModelBase
{
    public T ViewModel => (T)DataContext!;

    protected Tab(T viewModel) => DataContext = viewModel;
}