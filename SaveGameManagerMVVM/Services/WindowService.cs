using SaveGameManagerMVVM.Interfaces;
using SaveGameManagerMVVM.Viewmodels;
using System.Linq;
using System.Windows;

namespace SaveGameManagerMVVM.Services;

public class WindowService : IWindowService
{
    public WindowService(){}

    public void OpenWindow<T>(ViewModelBase viewModel, Window parent)
        where T: Window, new()
    {
        T window = new T();
        window.DataContext = viewModel;
        window.Owner = parent;
        window.ShowDialog();
    }

    public void CloseWindow<T>()
        where T : Window, new()
    {
        var window = Application.Current.Windows.OfType<T>().FirstOrDefault(x =>x.IsActive);

        if (window != null)
            window.Close();
    }
}
