using SaveGameManager.Interfaces;
using SaveGameManager.Viewmodels;
using SaveGameManager.Views;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace SaveGameManager.Services;

public class WindowService : IWindowService
{
    readonly Notifier? _notifier;
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

    public void OpenWindowDialog<T>(T viewModel, ViewModelBase parent)
    {
        var window = GetView(viewModel);
        ArgumentNullException.ThrowIfNull(window, nameof(window));
        window.DataContext = viewModel;
        window.Owner = GetView(parent);
        window.ShowDialog();
    }

    public void OpenWindow<T>(T viewModel)
    {
        var window = GetView(viewModel);
        ArgumentNullException.ThrowIfNull(window, nameof(window));
        window.DataContext = viewModel;
        window.Show();
        window.Focus();
    }

    public void CloseWindow<T>(T viewModel)
    {
        var window = GetView(viewModel);
        window?.Close();
    }

    internal static Window? GetViewInstance<T>() where T : Window, new() => Application.Current.Windows.OfType<T>().FirstOrDefault();
    internal static Window? GetView<T>(T viewModel) => viewModel switch
    {
        TextDialogViewModel => GetViewInstance<TextDialog>() ?? new TextDialog(),
        NotifyBoxYesNoViewModel => GetViewInstance<NotifyBoxYesNo>() ?? new NotifyBoxYesNo(),
        NotifyBoxViewModel => GetViewInstance<NotifyBox>() ?? new NotifyBox(),
        AboutViewModel => GetViewInstance<About>() ?? new About(),
        ProfileDialogViewModel => GetViewInstance<ProfileDialog>() ?? new ProfileDialog(),
        GitHubViewModel => GetViewInstance<GitHub>() ?? new GitHub(),
        MainViewModel => GetViewInstance<MainWindow>() ?? new MainWindow(),
        SettingsDialogViewModel => GetViewInstance<Settings>() ?? new Settings(),
        _ => throw new NotImplementedException()
    };
}
