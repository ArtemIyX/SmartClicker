using System.Windows;
using System.Windows.Controls;

namespace SmartClicker_WPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для SettingsBoxMS.xaml
    /// </summary>
    public partial class SettingsBoxMs : UserControl
    {
        public string TextValueMS
        {
            get => (string)GetValue(TextValueMSProperty);
            set => SetValue(TextValueMSProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextValueMSProperty =
            DependencyProperty.Register("TextValueMS", typeof(string), typeof(SettingsBoxMs), new PropertyMetadata("Seconds box: "));

        public string TextBoxValueMS
        {
            get => (string)GetValue(TextBoxValueMSProperty);
            set => SetValue(TextBoxValueMSProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextBoxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxValueMSProperty =
            DependencyProperty.Register("TextBoxValueMS", typeof(string), typeof(SettingsBoxMs), new PropertyMetadata("600"));


        public string TooltipValueMS
        {
            get => (string)GetValue(TooltipValueMSProperty);
            set => SetValue(TooltipValueMSProperty, value);
        }

        // Using a DependencyProperty as the backing store for TooltipValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TooltipValueMSProperty =
            DependencyProperty.Register("TooltipValueMS", typeof(string), typeof(SettingsBoxMs), new PropertyMetadata("Timeout"));

        public SettingsBoxMs()
        {
            InitializeComponent();
        }
    }
}
