using SmartClicker_WPF.Interfaces;
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
    /// Логика взаимодействия для ClickerSettings.xaml
    /// </summary>
    public partial class ClickerSettingsWindow : Window, ISettingsWindow
    {
        public ClickerSettingsVM ViewModel => (ClickerSettingsVM)DataContext;
        public ClickerSettingsWindow(ClickerSettingsVM VM)
        {
            this.DataContext = VM;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void InsertSettings(SettingsJson settingsJson)
        {
            ViewModel.InsertSettings(settingsJson);
        }

        public void ModifySettings(SettingsJson settingsJson)
        {
            ViewModel.ModifySettings(settingsJson);
        }

        public void ShowAsDialog()
        {
            this.ShowDialog();
        }
    }
}
