using SaveGameManagerMVVM.Viewmodels;
using ToastNotifications;

namespace SaveGameManagerMVVM.Interfaces;

public interface IWindowService
{
    public enum Windows
    {
        Textdialog,
        About,
        ProfileDialog,
        MainWindow,
        GitHub,
        NotificationBox
    };
    public void NotifierInformation(string message);
    public void NotifierSuccess(string message);
    public void NotifierWarning(string message);
    public void NotifierError(string message);

    public void OpenWindowDialog(Windows win, ViewModelBase viewModel, Windows parent);
    public void OpenWindow(Windows win, ViewModelBase viewModel);
    public void CloseWindow(Windows win);
    public string OpenFolderWindow(string path);
}
