using SaveGameManagerMVVM.Interfaces;
using SaveGameManagerMVVM.Viewmodels;
using SaveGameManagerMVVM.Views;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms.Design;

namespace SaveGameManagerMVVM.Services;

public class WindowService : IWindowService
{
    public WindowService()
    {
        
    }

    public string OpenFolderWindow(string path = "")
    {
        var dialog = new System.Windows.Forms.FolderBrowserDialog();
        dialog.InitialDirectory = @"C:\";

        if (!string.IsNullOrEmpty(path) & Directory.Exists(path))
            dialog.InitialDirectory = path;


        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            return dialog.SelectedPath;

        return string.Empty;
    }

    public void OpenWindow(IWindowService.Windows win, ViewModelBase viewModel, IWindowService.Windows parent)
    {
        var window = GetView(win);
        ArgumentNullException.ThrowIfNull(window, nameof(window));
        window.DataContext = viewModel;
        window.Owner = GetView(parent, true);
        window.ShowDialog();
    }

    public void CloseWindow(IWindowService.Windows win)
    {
        var window = GetView(win, true);
        window?.Close();
    }

    internal static Window? GetViewInstance<T>() where T : Window, new() => Application.Current.Windows.OfType<T>().FirstOrDefault(x => x.IsActive);
    internal static Window? GetView(IWindowService.Windows windows, bool open = false) => windows switch
    {

        IWindowService.Windows.Textdialog => open ? GetViewInstance<TextDialog>() : new TextDialog(),
        IWindowService.Windows.About => open ? GetViewInstance<About>() : new About(),
        IWindowService.Windows.ProfileDialog => open ? GetViewInstance<ProfileDialog>() : new ProfileDialog(),
        IWindowService.Windows.MainWindow => GetViewInstance<MainWindow>(),
        _ => throw new NotImplementedException()
    };
}
