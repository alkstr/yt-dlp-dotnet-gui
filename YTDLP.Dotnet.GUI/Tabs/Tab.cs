﻿using Avalonia.Controls;
using YTDLP.Dotnet.GUI.ViewModels;

namespace YTDLP.Dotnet.GUI.Tabs
{
    public abstract class Tab<ViewModelT> : UserControl where ViewModelT : ViewModelBase
    {
        public ViewModelT ViewModel => (ViewModelT)DataContext!;

        protected Tab(ViewModelT viewModel) => DataContext = viewModel;
    }
}
