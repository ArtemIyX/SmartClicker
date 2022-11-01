using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Models
{
    public enum AdDetectType
    {
        TagName,        //a
        HasAttribute,   //target
        AttributeValue, //id="5"
        HasClass,       //img_ad
        ContainsUrl,     //googleads
        CSS,            //.h
        XPath           // //*[@id="id"]
    }

    [ObservableObject]
    public partial class AdDetect
    {
        public string? Value { get; set; }
        private static string GetHint(int selectedIndex)
        {
            switch ((AdDetectType)selectedIndex)
            {
                case AdDetectType.TagName:
                    return "div";
                case AdDetectType.HasAttribute:
                    return "href";
                case AdDetectType.AttributeValue:
                    return "id=\"wrapper\"";
                case AdDetectType.HasClass:
                    return "jar";
                case AdDetectType.ContainsUrl:
                    return "googleads";
                case AdDetectType.CSS:
                    return "#content";
                case AdDetectType.XPath:
                    return "//*[@id=\"id\"]\r\n    }\r\n";
            }
            return "";
        }
        [ObservableProperty]
        public string _hint = "div";

        public List<string> Types => Enum.GetValues<AdDetectType>().Select(x => x.ToString()).ToList();

        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get => _selectedIndex;

            set
            {
                _selectedIndex = value;
                Hint = GetHint(_selectedIndex);
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        public AdDetectType Type => (AdDetectType)(SelectedIndex);
    }
}
