using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Models
{
    public enum AdDetectType
    {
        Element,
        Tag,
        Attribute,
        Class,
        Url,
        CSS,
        XPath,
    }

    public class AdDetect
    {
        public string? Value { get; set; }
        public List<string> Types => Enum.GetValues<AdDetectType>().Select(x => x.ToString()).ToList();
        public int SelectedIndex { get; set; } = 0;
        public AdDetectType Type => (AdDetectType)(SelectedIndex);
    }
}
