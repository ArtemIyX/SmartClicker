﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SmartClicker_WPF.Finders;
using SmartClicker_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SmartClicker_WPF.Services
{
    public class WebTasker
    {
        private static string GoogleURL = @"https://www.google.com/";

        private WebDriver? _driver;
        private readonly int _loops;
        private readonly ICollection<string> _proxies;
        private readonly WebProxyType _proxyType;
        private readonly WebDriverType _webDriverType;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _useProxy;
        private readonly WebService _webService;
        private readonly string _site;
        private readonly string _keywords;
        private readonly string _driverPath;
        private readonly int _timeOut;
        private CancellationToken _cancellationToken;
        private Task _backgroundCheckTask;

        private List<string> _keys = new List<string>();

        public event Action<string> OnFinished;
        public event Action<string> OnLog;

        public int CancelCheckDelayMs { get; set; } = 500;
        public int FindCookieButtonTimeOutS { get; set; } = 30;
        public int FindSearchTimeOutS { get; set; } = 30;

        public WebTasker(CancellationToken cancellationToken, 
            WebService webService,
            string site,
            string keywords,
            string driverPath, 
            int timeOut, 
            WebDriverType webDriverType, 
            int loops)
        {
            _cancellationToken = cancellationToken;
            _webService = webService;
            _site = site;
            _keywords = keywords;
            _driverPath = driverPath;
            _timeOut = timeOut;
            _webDriverType = webDriverType;
            _loops = loops;
            _useProxy = false;
        }

        public WebTasker(CancellationToken cancellationToken, 
            WebService webService,
            string site,
            string keywords,
            string driverPath, 
            int timeOut, 
            WebDriverType webDriverType, 
            int loops,
            ICollection<string> proxies, 
            WebProxyType proxyType,
            string username, 
            string password) 
            : this(cancellationToken, webService, site, keywords, driverPath, timeOut, webDriverType, loops)
        {
            _proxies = proxies;
            _proxyType = proxyType;
            _username = username;
            _password = password;
            _useProxy = true;
        }

        private List<string> SplitKeyWords()
        {
            return null;
        }

        public async Task Run()
        {
            _backgroundCheckTask = Task.Run(() => { BackgroundChecking(); });

            InitDriver(0);

            if (_driver == null)
                throw new Exception("Driver has not been initialized");

            _driver.Navigate().GoToUrl(GoogleURL); // go to google url
            await AccepCookiesGoogle();
        }
        private async Task SearchFor(string query)
        {
            OnLog.Invoke("Looking google search input...");
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, FindCookieButtonTimeOutS));
            IWebElement? searchInput = wait.Until(drv =>
            {
                return GoogleFinder.GetMainGoogleSearchInput(drv);
            });
            if (searchInput == null)
            {
                FinishWork("Can not find google search input");
                return;
            }
            
        }
        private async Task AccepCookiesGoogle()
        {
            OnLog.Invoke("Looking for cookies button...");
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, FindCookieButtonTimeOutS));
            IWebElement? cookieButton = wait.Until(drv =>
            {
                return GoogleFinder.GetAcceptCookieButton(drv);
            });
            if (cookieButton != null)
            {
                await Task.Delay(500);
                cookieButton.Click();
                OnLog.Invoke("Accepted cookies");
            }
            else
            {
                OnLog.Invoke("Cookies button not found");
            }
        }

        public void InitDriver(int index = 0)
        {
            if (_useProxy)
            {
                if (!string.IsNullOrEmpty(_username) &&
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
        private void FinishWork(string reason)
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver = null;
                OnFinished.Invoke(reason);
            }
        }
        private void CheckCancel()
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                _cancellationToken.ThrowIfCancellationRequested();
            }
        }
        private void BackgroundChecking()
        {
            while (true)
            {
                try
                {
                    CheckCancel();
                    Task.Delay(CancelCheckDelayMs);
                }
                catch (Exception ex)
                {
                    FinishWork("Aborted");
                    break;
                }
            }
        }


    }
}
