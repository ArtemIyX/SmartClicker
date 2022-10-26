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
        Class,
        Url
    }

    public class AdDetect
    {
        public AdDetectType Type { get; set; }
        public List<string>? Values { get; set; }
    }
}
