using SaveGameManagerMVVM.Viewmodels;

namespace SaveGameManagerMVVM.Interfaces;

public interface IWindowService
{
    public enum Windows
    {
        Textdialog,
        About,
        ProfileDialog,
        MainWindow,
        GitHub
    };
    public void OpenWindowDialog(Windows win, ViewModelBase viewModel, Windows parent);
    public void OpenWindow(Windows win, ViewModelBase viewModel);
    public void CloseWindow(Windows win);

    public string OpenFolderWindow(string path);
}
