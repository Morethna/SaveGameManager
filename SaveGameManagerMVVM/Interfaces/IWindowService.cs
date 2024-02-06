using SaveGameManagerMVVM.Viewmodels;

namespace SaveGameManagerMVVM.Interfaces;

public interface IWindowService
{
    public enum Windows
    {
        Textdialog,
        About,
        ProfileDialog,
        MainWindow
    };
    public void OpenWindow(Windows win, ViewModelBase viewModel, Windows parent);
    public void CloseWindow(Windows win);

    public string OpenFolderWindow(string path);
}
