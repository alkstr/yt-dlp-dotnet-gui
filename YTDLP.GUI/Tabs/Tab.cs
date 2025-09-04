using Avalonia.Controls;
using YTDLP.GUI.ViewModels;

namespace YTDLP.GUI.Tabs;

public abstract class Tab<T> : UserControl where T : ViewModelBase
{
    public T ViewModel => (T)DataContext!;

    protected Tab(T viewModel) => DataContext = viewModel;
}