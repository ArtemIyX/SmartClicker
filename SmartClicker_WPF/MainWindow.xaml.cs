using SmartClicker_WPF.Services;
using SmartClicker_WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartClicker_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainVM viewmodel => (MainVM)DataContext;
        private SettingsService _settingsService;
        public MainWindow(SettingsService settingsService, MainVM mainVM)
        {
            _settingsService = settingsService;
            this.DataContext = mainVM;
            InitializeComponent();
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewmodel.Tasker?.FinishWork("Window is closing");
            _settingsService.SaveSettingsObject(viewmodel.SettingsJson);
        }
    }
}
