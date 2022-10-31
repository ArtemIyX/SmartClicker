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
        /*Edge, Don't support http proxy with auth*/
        Chrome,
        Firefox
    }
    public class SettingsJson
    {
        public string? ChromeDriverPath { get; set; }
        public string? EdgeDriverPath { get; set; }
        public string? FirefoxDriverPath { get; set; }
    }
}
