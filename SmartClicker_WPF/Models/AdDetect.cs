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
        Url
    }

    public class AdDetect
    {
        public AdDetectType Type { get; set; }
        public List<DetectValue>? Values { get; set; }

        public string DisplayValue
        {
            get
            {
                if (Values == null)
                    return "none";
                if (Values.Count == 0)
                    return "";
                if (Values.Count == 1)
                    return Values[0].Header;
                return Values[0].Header + "...";
            }
        }
        public string DisplayType => Type.ToString();
    }
}
