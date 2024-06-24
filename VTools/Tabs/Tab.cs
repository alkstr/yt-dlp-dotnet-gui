using Avalonia.Controls;
using VTools.ViewModels;

namespace VTools.Tabs
{
    public abstract class Tab<ViewModelT> : UserControl where ViewModelT : ViewModelBase
    {
        public ViewModelT ViewModel => (ViewModelT)DataContext!;

        protected Tab(ViewModelT viewModel) => DataContext = viewModel;
    }
}
