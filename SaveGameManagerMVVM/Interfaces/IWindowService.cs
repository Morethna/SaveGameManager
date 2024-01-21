using SaveGameManagerMVVM.Viewmodels;
using System.Windows;

namespace SaveGameManagerMVVM.Interfaces;

public interface IWindowService
{
    public void OpenWindow<T>(ViewModelBase viewModel, Window parent)
        where T : Window, new();
    public void CloseWindow<T>()
        where T : Window, new();
}
