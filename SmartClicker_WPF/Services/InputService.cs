using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Services
{
    public class InputService
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private bool IsOnlyNumberText(string text)
        {
            return _regex.IsMatch(text);
        }
    }
}
