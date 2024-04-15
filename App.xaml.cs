using Microsoft.Extensions.DependencyInjection;
using SaveGameManager.Interfaces;
using SaveGameManager.Services;
using SaveGameManager.Viewmodels;
using System;
using System.Windows;
using System.Windows.Threading;

namespace SaveGameManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IDataService, DataService>();
            services.AddSingleton<IDirectoryService, DirectoryService>();
            services.AddSingleton<IUiSettingsService, UiSettingsService>();
            services.AddSingleton<IWindowService, WindowService>();
            services.AddSingleton<IGitHubService, GitHubService>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<TextDialogViewModel>();
            services.AddSingleton<AboutViewModel>();
            services.AddSingleton<ProfileDialogViewModel>();
            services.AddSingleton<GitHubViewModel>();
            services.AddSingleton<NotifyBoxYesNoViewModel>();
            services.AddSingleton<NotifyBoxViewModel>();
            services.AddSingleton<SettingsDialogViewModel>();

            services.AddSingleton(sp => new MainWindow()
            {
                DataContext = sp.GetRequiredService<MainViewModel>()
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var gitHub = _serviceProvider.GetService<IGitHubService>();
            gitHub?.CheckGitHubNewerVersion();

            var startForm = _serviceProvider.GetRequiredService<MainWindow>();
            startForm.Show();
            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            var startForm = _serviceProvider.GetRequiredService<IDataService>();
            startForm.SaveConfigAsync();
            base.OnExit(e);
        }
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var windowService = _serviceProvider.GetService<IWindowService>();
            var viewModel = _serviceProvider.GetService<NotifyBoxViewModel>();

            if (windowService != null && viewModel != null) 
            {
                viewModel.Message = e.Exception.Message;
                viewModel.Title = "Error";
                windowService.OpenWindow(viewModel);
            }
            else
                MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

}
