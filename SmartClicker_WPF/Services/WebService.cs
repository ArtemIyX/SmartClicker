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

namespace SmartClicker_WPF.Services
{
    public class WebService
    {
        public int TimeOutSec { get; set; }

        public WebService()
        {
            TimeOutSec = 30;
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
            OpenQA.Selenium.Proxy res = new OpenQA.Selenium.Proxy();
            res.Kind = ProxyKind.Manual;
            res.IsAutoDetect = false;

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

        public WebDriver CreateWebDriver(string path, WebDriverType webDriverType)
        {
            switch (webDriverType)
            {
                case WebDriverType.Chrome:
                    return CreateChromeDriver(path);
                case WebDriverType.Firefox:
                    return CreateFirefoxDriver(path);
            }
            return new ChromeDriver();
        }
        public WebDriver CreateWebDriverWithProxy(string path, WebDriverType webDriverType, WebProxyType webProxyType, string ip)
        {
            switch (webDriverType)
            {
                case WebDriverType.Chrome:
                    return CreateChromeDriverWithProxy(path, webProxyType, ip);
                case WebDriverType.Firefox:
                    return CreateFirefoxDriverWithProxy(path, webProxyType, ip);
            }
            return new ChromeDriver();
        }

        public WebDriver CreateWebDriverWithPrivateProxy(string path, WebDriverType webDriverType,
            WebProxyType webProxyType, string ip,
            string username,
            string password)
        {
            switch (webDriverType)
            {
                case WebDriverType.Chrome:
                    return CreateChromeDriverWithPrivateProxy(path, webProxyType, ip, username, password);
                case WebDriverType.Firefox:
                    return CreateFirefoxDriverWithPrivateProxy(path, webProxyType, ip, username, password);
            }
            return new ChromeDriver();
        }

        private ChromeOptions CreateChromeOptions()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-popup-blocking");
            options.AddExcludedArgument("enable-automation");
            return options;
        }

        public ChromeDriver CreateChromeDriver(string path)
        {
            var driver = new ChromeDriver(path, CreateChromeOptions(), TimeSpan.FromSeconds(TimeOutSec));
            return driver;
        }

        public ChromeDriver CreateChromeDriverWithProxy(string path, WebProxyType webProxyType, string ip)
        {
            var options = CreateChromeOptions();
            OpenQA.Selenium.Proxy proxy = CreateProxy(webProxyType, ip);
            options.Proxy = proxy;
            var driver = new ChromeDriver(path, options, TimeSpan.FromSeconds(TimeOutSec));
            return driver;
        }

        public ChromeDriver CreateChromeDriverWithPrivateProxy(string path,
            WebProxyType webProxyType, string ip,
            string username,
            string password)
        {
            var options = CreateChromeOptions();
            string[] ip_port = ip.Split(":");
            options.AddHttpProxy(ip_port[0], int.Parse(ip_port[1]), username, password);
            var driver = new ChromeDriver(path, options, TimeSpan.FromSeconds(TimeOutSec));
            return driver;
        }

        public FirefoxDriver CreateFirefoxDriver(string path)
        {
            var options = new FirefoxOptions();
            var driver = new FirefoxDriver(path, options, TimeSpan.FromSeconds(TimeOutSec));
            return driver;
        }

        public FirefoxDriver CreateFirefoxDriverWithProxy(string path, WebProxyType webProxyType, string ip)
        {
            var options = new FirefoxOptions();
            OpenQA.Selenium.Proxy proxy = CreateProxy(webProxyType, ip);
            var driver = new FirefoxDriver(path, options, TimeSpan.FromSeconds(TimeOutSec));
            return driver;
        }

        public FirefoxDriver CreateFirefoxDriverWithPrivateProxy(string path,
            WebProxyType webProxyType, string ip,
            string username,
            string password)
        {
            var options = new FirefoxOptions();
            var driver = new FirefoxDriver(path, options, TimeSpan.FromSeconds(TimeOutSec));
            SeleniumProxyAuthentication.ProxyProtocols libProtocol = ToLibProtocol(webProxyType);
            string[] ip_port = ip.Split(":");
            string proxyStr = $"{ip_port[0]}:{ip_port[1]}:{username}:{password}";
            driver.AddProxyAuthenticationExtension(new SeleniumProxyAuthentication.Proxy(libProtocol, proxyStr));
            return driver;
        }

    }
}
