using SaveGameManagerMVVM.Interfaces;
using SaveGameManagerMVVM.Viewmodels;
using SaveGameManagerMVVM.Views;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace SaveGameManagerMVVM.Services;

public class WindowService : IWindowService
{
    Notifier? _notifier;
    public WindowService()
    {
        _notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 30);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(2),
                maximumNotificationCount: MaximumNotificationCount.FromCount(3));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });
    }

    public void NotifierInformation(string message) => _notifier.ShowInformation(message);
    public void NotifierSuccess(string message) => _notifier.ShowSuccess(message);
    public void NotifierWarning(string message) => _notifier.ShowWarning(message);
    public void NotifierError(string message) => _notifier.ShowError(message);

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

    public void OpenWindowDialog(IWindowService.Windows win, ViewModelBase viewModel, IWindowService.Windows parent)
    {
        var window = GetView(win);
        ArgumentNullException.ThrowIfNull(window, nameof(window));
        window.DataContext = viewModel;
        window.Owner = GetView(parent, true);
        window.ShowDialog();
    }

    public void OpenWindow(IWindowService.Windows win, ViewModelBase viewModel)
    {
        var window = GetView(win);
        ArgumentNullException.ThrowIfNull(window, nameof(window));
        window.DataContext = viewModel;
        window.Show();
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
        IWindowService.Windows.NotificationBox => open ? GetViewInstance<NotificationBox>() : new NotificationBox(),
        IWindowService.Windows.About => open ? GetViewInstance<About>() : new About(),
        IWindowService.Windows.ProfileDialog => open ? GetViewInstance<ProfileDialog>() : new ProfileDialog(),
        IWindowService.Windows.GitHub => open ? GetViewInstance<GitHub>() : new GitHub(),
        IWindowService.Windows.MainWindow => GetViewInstance<MainWindow>(),
        _ => throw new NotImplementedException()
    };
}
