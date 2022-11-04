using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Models
{
    public enum WebProxyType
    {
        Https,
        Socks4,
        Socks5
    }
    public enum WebDriverType
    {
        /*Edge, Don't support https proxy with auth*/
        Chrome,
        Firefox
    }
    public class SettingsJson
    {
        public string? ChromeDriverPath { get; set; }
        public string? FirefoxDriverPath { get; set; }

        public int TimeOut_CookieButton { get; set; }
        public int TimeOut_SearchBar { get; set; }
        public int TimeOut_SearchButton { get; set; }
        //public int TimeOut_SearchSite { get; set; }
        public int TimeOut_NavTable { get; set; }
        public int TimeOut_Page { get; set; }
        public int TimeOut_PageLoad { get; set; }

        public int TimeOut_FindLink { get; set; }
        public int TimeOut_LinkClick { get; set; }

        public int MaxPageCount { get; set; }

        public int TimeOut_ProxyCheckerMs { get; set; }
        public int DelayBetweenActivityMs { get; set; }

        public string? GoogleDismisButtonId { get; set; }

        public bool HideConsole { get; set; }
        public bool HideBrowser { get; set; }
        public bool RandomDelayBetweenActivity { get; set; }
    }
}
