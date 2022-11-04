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
        private static readonly Regex OnlyNumberRegex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        public bool IsOnlyNumberText(string text) => OnlyNumberRegex.IsMatch(text);

        public List<string> SplitKeyWords(string input) => input.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

    }
}
