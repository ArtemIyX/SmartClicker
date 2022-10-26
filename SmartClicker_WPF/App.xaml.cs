using Microsoft.Extensions.DependencyInjection;
using SmartClicker_WPF.Services;
using SmartClicker_WPF.ViewModels;
using SmartClicker_WPF.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SmartClicker_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            /* string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
             MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);*/
            var errorBox = new ErrorBox("Error", e.Exception.Message);
            errorBox.ShowDialog();
            e.Handled = true;
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<FooManager>();
            services.AddSingleton<MainWindow>();
            services.AddScoped<SettingsService>();
            services.AddScoped<InputService>();
            services.AddScoped<MainVM>();
            services.AddScoped<NewDetectVM>();
            services.AddScoped<NewDetectWindow>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
