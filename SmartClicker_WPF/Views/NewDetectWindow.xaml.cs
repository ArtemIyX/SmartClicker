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
    /// Логика взаимодействия для NewDetectWindow.xaml
    /// </summary>
    public partial class NewDetectWindow : Window
    {
        private NewDetectVM ViewModel => (NewDetectVM)DataContext;
        public NewDetectWindow(NewDetectVM VM)
        {
            this.DataContext = VM;
            InitializeComponent();
        }

    }
}
