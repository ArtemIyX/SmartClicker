using SmartClicker_WPF.Models;
using SmartClicker_WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartClicker_WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для DriverSettingsWindow.xaml
    /// </summary>
    public partial class DriverSettingsWindow : Window
    {
        public DriverSettingsVM ViewModel => (DriverSettingsVM)DataContext;
        public DriverSettingsWindow(DriverSettingsVM VM)
        {
            this.DataContext = VM;
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
