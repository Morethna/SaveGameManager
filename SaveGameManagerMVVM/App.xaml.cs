using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaveGameManager.Handler;
using SaveGameManagerMVVM.Interfaces;
using SaveGameManagerMVVM.Services;
using SaveGameManagerMVVM.Viewmodels;
using System;
using System.Windows;
using System.Windows.Threading;

namespace SaveGameManagerMVVM
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
            services.AddSingleton<MainViewModel>();

            services.AddSingleton(sp => new MainWindow()
            {
                DataContext = sp.GetRequiredService<MainViewModel>()
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var startForm = _serviceProvider.GetRequiredService<MainWindow>();
            startForm.Show();
            base.OnStartup(e);
        }
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

}
