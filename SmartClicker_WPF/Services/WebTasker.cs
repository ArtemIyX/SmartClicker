using OpenQA.Selenium;
using SmartClicker_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Services
{
    public class WebTasker
    {
        private WebDriver _driver;
        private readonly int _loops;
        private readonly ICollection<string> _proxies;
        private readonly WebProxyType _proxyType;
        private readonly WebDriverType _webDriverType;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _useProxy;
        private readonly WebService _webService;
        private readonly string _driverPath;
        private readonly int _timeOut;

        public WebTasker(WebService webService, string driverPath, int timeOut, WebDriverType webDriverType, int loops)
        {
            _webService = webService;
            _driverPath = driverPath;
            _timeOut = timeOut;
            _webDriverType = webDriverType;
            _loops = loops;
            _useProxy = false;
        }

        public WebTasker(WebService webService, string driverPath, int timeOut, WebDriverType webDriverType, int loops,
            ICollection<string> proxies, WebProxyType proxyType,
            string username, string password)
        {
            _webService = webService;
            _webDriverType = webDriverType;
            _driverPath = driverPath;
            _timeOut = timeOut;
            _loops = loops;
            _proxies = proxies;
            _proxyType = proxyType;
            _username = username;
            _password = password;
            _useProxy = true;
        }

        public void InitDriver(int index = 0)
        {
            if (_useProxy)
            {
                if(!string.IsNullOrEmpty(_username) &&
                    !string.IsNullOrEmpty(_password))
                {
                    _driver = _webService.CreateWebDriverWithPrivateProxy(_driverPath, 
                        _webDriverType, 
                        _proxyType, 
                        _proxies.ElementAt(index), 
                        _username, 
                        _password);
                }
                else
                {
                    _driver = _webService.CreateWebDriverWithProxy(_driverPath,
                        _webDriverType,
                        _proxyType,
                        _proxies.ElementAt(index));
                }
            }
            else
            {
                _driver = _webService.CreateWebDriver(_driverPath, _webDriverType);
            }
        }

        public async Task Start()
        {
            InitDriver(0);
            _driver.Navigate().GoToUrl("http://azenv.net/");
        }
    }
}
