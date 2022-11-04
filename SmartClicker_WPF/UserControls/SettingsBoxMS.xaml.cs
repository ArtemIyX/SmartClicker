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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartClicker_WPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для SettingsBoxMS.xaml
    /// </summary>
    public partial class SettingsBoxMS : UserControl
    {
        public string TextValueMS
        {
            get { return (string)GetValue(TextValueMSProperty); }
            set { SetValue(TextValueMSProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextValueMSProperty =
            DependencyProperty.Register("TextValueMS", typeof(string), typeof(SettingsBoxMS), new PropertyMetadata("Seconds box: "));

        public string TextBoxValueMS
        {
            get { return (string)GetValue(TextBoxValueMSProperty); }
            set { SetValue(TextBoxValueMSProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextBoxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxValueMSProperty =
            DependencyProperty.Register("TextBoxValueMS", typeof(string), typeof(SettingsBoxMS), new PropertyMetadata("600"));


        public string TooltipValueMS
        {
            get { return (string)GetValue(TooltipValueMSProperty); }
            set { SetValue(TooltipValueMSProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TooltipValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TooltipValueMSProperty =
            DependencyProperty.Register("TooltipValueMS", typeof(string), typeof(SettingsBoxMS), new PropertyMetadata("Timeout"));

        public SettingsBoxMS()
        {
            InitializeComponent();
        }
    }
}
