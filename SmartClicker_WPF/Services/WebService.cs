using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chrome.ChromeDriverExtensions;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using SeleniumProxyAuthentication;
using SmartClicker_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proxy = OpenQA.Selenium.Proxy;

namespace SmartClicker_WPF.Services
{
    public class WebService
    {
        public int TimeOutSec { get; set; }

        public WebService()
        {
            TimeOutSec = 120;
        }

        private SeleniumProxyAuthentication.ProxyProtocols ToLibProtocol(WebProxyType webProxyType)
        {
            switch (webProxyType)
            {
                case WebProxyType.Https:
                    return SeleniumProxyAuthentication.ProxyProtocols.HTTPS;
                case WebProxyType.Socks4:
                    return SeleniumProxyAuthentication.ProxyProtocols.SOCKS5;
                case WebProxyType.Socks5:
                    return SeleniumProxyAuthentication.ProxyProtocols.SOCKS5;
            }

            return SeleniumProxyAuthentication.ProxyProtocols.HTTP;
        }

        private OpenQA.Selenium.Proxy CreateProxy(WebProxyType type, string ip)
        {
            OpenQA.Selenium.Proxy res = new OpenQA.Selenium.Proxy
            {
                Kind = ProxyKind.Manual,
                IsAutoDetect = false
            };

            switch (type)
            {
                case WebProxyType.Https:
                    res.HttpProxy = ip;
                    res.SslProxy = ip;
                    break;
                case WebProxyType.Socks4:
                    res.SocksProxy = ip;
                    res.SocksVersion = 4;
                    break;
                case WebProxyType.Socks5:
                    res.SocksProxy = ip;
                    res.SocksVersion = 5;
                    break;
            }

            return res;
        }

        public WebDriver CreateWebDriver(string path, 
            WebDriverType webDriverType,
            bool hideBrowser = false,
            bool hideConsole = false)
        {
            switch (webDriverType)
            {
                case WebDriverType.Chrome:
                    return CreateChromeDriver(path, hideBrowser, hideConsole);
                case WebDriverType.Firefox:
                    return CreateFirefoxDriver(path, hideBrowser, hideConsole);
            }

            return new ChromeDriver();
        }

        public WebDriver CreateWebDriverWithProxy(string path,
            WebDriverType webDriverType,
            WebProxyType webProxyType,
            string ip,
            bool hideBrowser = false,
            bool hideConsole = false)
        {
            switch (webDriverType)
            {
                case WebDriverType.Chrome:
                    return CreateChromeDriverWithProxy(path, webProxyType, ip, hideBrowser, hideConsole);
                case WebDriverType.Firefox:
                    return CreateFirefoxDriverWithProxy(path, webProxyType, ip, hideBrowser, hideConsole);
            }

            return new ChromeDriver();
        }

        public WebDriver CreateWebDriverWithPrivateProxy(string path, WebDriverType webDriverType,
            WebProxyType webProxyType, string ip,
            string username,
            string password,
            bool hideBrowser = false,
            bool hideConsole = false)
        {
            switch (webDriverType)
            {
                case WebDriverType.Chrome:
                    return CreateChromeDriverWithPrivateProxy(path, webProxyType, ip, username, password, hideBrowser, hideConsole);
                case WebDriverType.Firefox:
                    return CreateFirefoxDriverWithPrivateProxy(path, webProxyType, ip, username, password, hideBrowser, hideConsole);
            }

            return new ChromeDriver();
        }

        private ChromeOptions CreateChromeOptions(bool hideBrowser)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--disable-popup-blocking");
            options.AddExcludedArgument("enable-automation");
            if (hideBrowser)
            {
                options.AddArguments(new List<string>
                {
                    "--silent-launch",
                    "--no-startup-window",
                    "no-sandbox",
                    "headless"
                });
            }

            return options;
        }

        private FirefoxOptions CreateFireFoxOptions(bool hideBrowser)
        {
            FirefoxOptions options = new FirefoxOptions();
            if (hideBrowser)
            {
                /*options.AddArguments(new List<string>
                {
                    "--silent-launch",
                    "--no-startup-window",
                    "no-sandbox",
                    "headless"
                });*/
                options.AddArguments("--headless");
            }

            return options;
        }

        public ChromeDriver CreateChromeDriver(string path, bool hideBrowser = false, bool hideConsole = false)
        {
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(path);
            chromeDriverService.HideCommandPromptWindow = hideConsole;
            return new ChromeDriver(chromeDriverService,
                CreateChromeOptions(hideBrowser), TimeSpan.FromSeconds(TimeOutSec));
            ;
        }

        public ChromeDriver CreateChromeDriverWithProxy(string path,
            WebProxyType webProxyType, string ip,
            bool hideBrowser = false, bool hideConsole = false)
        {
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(path);
            chromeDriverService.HideCommandPromptWindow = hideConsole;

            ChromeOptions options = CreateChromeOptions(hideBrowser);
            OpenQA.Selenium.Proxy proxy = CreateProxy(webProxyType, ip);

            options.Proxy = proxy;

            return new ChromeDriver(chromeDriverService,
                options,
                TimeSpan.FromSeconds(TimeOutSec));
            ;
        }

        public ChromeDriver CreateChromeDriverWithPrivateProxy(string path,
            WebProxyType webProxyType, string ip,
            string username,
            string password,
            bool hideBrowser = false, bool hideConsole = false)
        {
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(path);
            chromeDriverService.HideCommandPromptWindow = hideConsole;

            ChromeOptions options = CreateChromeOptions(hideBrowser);
            if (webProxyType == WebProxyType.Https)
            {
                string[] ipPort = ip.Split(":");
                options.AddHttpProxy(ipPort[0], int.Parse(ipPort[1]), username, password);
            }
            else
            {
                OpenQA.Selenium.Proxy proxy = CreateProxy(webProxyType, ip);
                proxy.SocksUserName = username;
                proxy.SocksPassword = password;
                options.Proxy = proxy;
            }

            return new ChromeDriver(chromeDriverService,
                options,
                TimeSpan.FromSeconds(TimeOutSec));
            ;
        }

        public FirefoxDriver CreateFirefoxDriver(string path,
            bool hideBrowser = false, bool hideConsole = false)
        {
            FirefoxDriverService firefoxDriverService = FirefoxDriverService.CreateDefaultService(path);
            firefoxDriverService.HideCommandPromptWindow = hideConsole;
            FirefoxOptions options = CreateFireFoxOptions(hideBrowser);
            return new FirefoxDriver(firefoxDriverService,
                options, TimeSpan.FromSeconds(TimeOutSec));
        }

        public FirefoxDriver CreateFirefoxDriverWithProxy(string path,
            WebProxyType webProxyType, string ip,
            bool hideBrowser = false, bool hideConsole = false)
        {
            FirefoxDriverService firefoxDriverService = FirefoxDriverService.CreateDefaultService(path);
            firefoxDriverService.HideCommandPromptWindow = hideConsole;
            FirefoxOptions options = CreateFireFoxOptions(hideBrowser);
            OpenQA.Selenium.Proxy proxy = CreateProxy(webProxyType, ip);
            return new FirefoxDriver(firefoxDriverService, options, TimeSpan.FromSeconds(TimeOutSec));
        }

        public FirefoxDriver CreateFirefoxDriverWithPrivateProxy(string path,
            WebProxyType webProxyType, string ip,
            string username,
            string password,
            bool hideBrowser = false, bool hideConsole = false)
        {
            FirefoxDriverService firefoxDriverService = FirefoxDriverService.CreateDefaultService(path);
            firefoxDriverService.HideCommandPromptWindow = hideConsole;
            FirefoxOptions options = CreateFireFoxOptions(hideBrowser);
            FirefoxDriver driver = new FirefoxDriver(path, options, TimeSpan.FromSeconds(TimeOutSec));

            string[] ipPort = ip.Split(":");
            string proxyStr = $"{ipPort[0]}:{ipPort[1]}:{username}:{password}";

            driver.AddProxyAuthenticationExtension(
                new SeleniumProxyAuthentication.Proxy(ToLibProtocol(webProxyType), proxyStr));
            return driver;
        }
    }
}