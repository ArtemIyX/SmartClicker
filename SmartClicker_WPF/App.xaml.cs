using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenQA.Selenium.DevTools.V104.Database;
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
            var errorBox = new ErrorBox("Error", e.Exception.Message);
            errorBox.ShowDialog();
            e.Handled = true;
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<FooManager>();
            services.AddSingleton<MainWindow>();
            services.AddTransient<SettingsService>();
            services.AddTransient<InputService>();
            services.AddTransient<ProxyService>();
            services.AddTransient<AdDetectService>();
            services.AddTransient<MainVM>();
            services.AddTransient<NewDetectVM>();
            services.AddTransient<NewDetectWindow>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
