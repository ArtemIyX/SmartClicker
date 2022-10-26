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
    /// Логика взаимодействия для ErrorBox.xaml
    /// </summary>
    public partial class ErrorBox : Window
    {
        private ErrorVM viewmodel => (ErrorVM)DataContext;
        public ErrorBox(string ErrorTitle, string ErrorMsg)
        {
            this.DataContext = new ErrorVM(ErrorTitle, ErrorMsg);
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
