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
    /// Логика взаимодействия для SettingsBoxS.xaml
    /// </summary>
    public partial class SettingsBoxS : UserControl
    {

        public string TextValueS
        {
            get { return (string)GetValue(TextValueSProperty); }
            set { SetValue(TextValueSProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextValueSProperty =
            DependencyProperty.Register("TextValueS", typeof(string), typeof(SettingsBoxS), new PropertyMetadata("Seconds box: "));

        public string TextBoxValueS
        {
            get { return (string)GetValue(TextBoxValueSProperty); }
            set { SetValue(TextBoxValueSProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextBoxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxValueSProperty =
            DependencyProperty.Register("TextBoxValueS", typeof(string), typeof(SettingsBoxS), new PropertyMetadata("60"));


        public string TooltipValueS
        {
            get { return (string)GetValue(TooltipValueSProperty); }
            set { SetValue(TooltipValueSProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TooltipValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TooltipValueSProperty =
            DependencyProperty.Register("TooltipValueS", typeof(string), typeof(SettingsBoxS), new PropertyMetadata("Timeout"));


        public SettingsBoxS()
        {
            InitializeComponent();
        }
    }
}
