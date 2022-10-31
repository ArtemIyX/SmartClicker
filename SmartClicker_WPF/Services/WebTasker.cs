﻿using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SmartClicker_WPF.Extensions;
using SmartClicker_WPF.Finders;
using SmartClicker_WPF.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly InputService _inputService;
        private readonly string _site;
        private readonly string _keywords;
        private readonly string _driverPath;
        private readonly int _timeOut;

        private Random _rand = new Random();
        private const int _minDelay = 250;
        private const int _maxDelay = 750;
        private int randDelay() => _rand.Next(_minDelay, _maxDelay);
        private List<string> _keys = new List<string>();
        private int _pageIndex = 1;

        public event Action<string> OnFinished;
        public event Action<string> OnLog;

        //TODO: To struct
        //Every N ms will check if we need to end task
        public int CancelCheckDelayMs { get; set; } = 500;
        //Find cookie button on google.com
        public int FindCookieButtonTimeOutS { get; set; } = 30;
        //Find search bar on google.com
        public int FindSearchBarTimeOutS { get; set; } = 30;
        //Find search button on google.com
        public int FindSearchButtonTimeOutS { get; set; } = 30;
        //
        public int WebSiteSearchTimeOutS { get; set; } = 6;
        public int NavTableSearchTiemOutS { get; set; } = 5;
        public int PageSearchTimeOutS { get; set; } = 5;
        public int MaxPageCount { get; set; } = 10;

        public WebTasker(
            WebService webService,
            InputService inputService,
            string site,
            string keywords,
            string driverPath,
            int timeOut,
            WebDriverType webDriverType,
            int loops)
        {
            _webService = webService;
            _inputService = inputService;
            _site = site;
            _keywords = keywords;
            _keys = _inputService.SplitKeyWords(_keywords);
            _driverPath = driverPath;
            _timeOut = timeOut;
            _webDriverType = webDriverType;
            _loops = loops;
            _useProxy = false;
        }

        public WebTasker(
            WebService webService,
            InputService inputService,
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
            : this(webService, inputService, site, keywords, driverPath, timeOut, webDriverType, loops)
        {
            _proxies = proxies;
            _proxyType = proxyType;
            _username = username;
            _password = password;
            _useProxy = true;
        }

        // Main function
        public async Task Run()
        {

            InitDriver(0);

            if (_driver == null)
                throw new Exception("Driver has not been initialized");

            _driver.Navigate().GoToUrl(GoogleURL);
            await AccepGoogleCookies();
            string selectedKey = _keys.RandomElement();
            await TypeSearchingQeury(selectedKey);
            await PressSearchingButton();
            await GoOnWebSite();
            await DoSomeActivityFor(_timeOut);
        }


        private async Task DoSomeActivityFor(int seconds)
        {
            OnLog.Invoke($"Doing some activity in {_driver.Url} for {seconds}s...");
            ActivityMaker activityMaker = new ActivityMaker(_driver);
            try
            {
                await activityMaker.DoActivityFor(seconds);
            }
            catch (OperationCanceledException ex)
            {
                OnLog.Invoke("Time is up, looking for advertising..");
            }
        }

        // Try find site by search result
        private async Task GoOnWebSite()
        {
            OnLog.Invoke($"Searching for website {_site}...");

            while (true)
            {
                var (s1, nav_table) = await GetNavTable();
                if (!s1) return;

                // Find link on tihs page
                IWebElement? link = GoogleFinder.FindUrlInSearch(_driver, _site);
                // If no link - go to next page
                if (link == null)
                {
                    await OpenNextPage(nav_table);
                }
                else
                {
                    await GoToSiteByLink(link);
                    return;
                }
            }
        }

        // Get navigation table of google (bottom numbers)
        private async Task<(bool, IWebElement?)> GetNavTable()
        {
            OnLog.Invoke($"Waiting for page {_pageIndex}...");

            // Wait until page is loaded
            IWebElement? nav_table = await _driver.FindElementAsync(NavTableSearchTiemOutS, drv => GoogleFinder.GetGooglePageTable(drv));

            if (nav_table == null)
            {
                FinishWork("First page is not loaded");
                return (false, null);
            }

            await Task.Delay(randDelay());
            OnLog.Invoke($"Page {_pageIndex} is loaded");
            return (true, nav_table);
        }

        //Scroll to link and click
        private async Task<bool> GoToSiteByLink(IWebElement link)
        {
            OnLog.Invoke($"Url found on page {_pageIndex}");
            if (await _driver.ScrollTo(link, _minDelay, _maxDelay))
            {
                return link.ClickSave();
            }
            return false;
        }

        // Try go to next page 
        private async Task<bool> OpenNextPage(IWebElement nav_table)
        {
            _pageIndex++;
            if (_pageIndex > MaxPageCount)
            {
                FinishWork($"We are already on the {_pageIndex} page and did not find {_site}");
                return false;
            }

            IWebElement? pageLink = await _driver.FindElementAsync(PageSearchTimeOutS, drv => GoogleFinder.GetGooglePageLink(nav_table, _pageIndex));

            if (pageLink == null)
            {
                FinishWork($"Can not found page({_pageIndex}) link");
                return false;
            }

            if (await _driver.ScrollTo(pageLink, _minDelay, _maxDelay))
            {
                return pageLink.ClickSave();
            }
            return true;
        }

        // Main page - press "Find in google"
        private async Task<bool> PressSearchingButton()
        {
            OnLog.Invoke("Looking for google search button...");

            IWebElement? searchButton = await _driver.FindElementAsync(FindSearchButtonTimeOutS, drv => GoogleFinder.GetMainGoogleSearchButton(drv));
            if (searchButton == null)
            {
                FinishWork("Can not find google search button");
                return false;
            }
            OnLog.Invoke("Found google search button");
            await Task.Delay(randDelay());
            return searchButton.ClickSave();
        }

        // Main page - type some query
        private async Task<bool> TypeSearchingQeury(string query)
        {
            OnLog.Invoke("Looking for google search input...");
            IWebElement? searchInput = await _driver.FindElementAsync(FindSearchBarTimeOutS, drv => GoogleFinder.GetMainGoogleSearchInput(drv));

            if (searchInput == null)
            {
                FinishWork("Can not find google search input");
                return false;
            }

            OnLog.Invoke("Found google search input");

            searchInput.SendKeysSave(query);
            if (_driver.ClickOnBlankArea())
            {
                await Task.Delay(randDelay());
                return true;
            }
            return false; 
            
        }

        // Accept google cookies
        private async Task AccepGoogleCookies()
        {
            OnLog.Invoke("Looking for cookies button...");
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, FindCookieButtonTimeOutS));
            IWebElement? cookieButton = await _driver.FindElementAsync(FindCookieButtonTimeOutS, drv => GoogleFinder.GetAcceptCookieButton(drv));

            if (cookieButton != null)
            {
                await Task.Delay(randDelay());
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

        public void FinishWork(string reason)
        {
            try
            {
                if (_driver != null)
                {
                    _driver.Manage().Cookies.DeleteAllCookies();
                    _driver.Quit();
                    _driver = null;
                    OnFinished.Invoke(reason);
                }
            }
            catch
            {

            }
        }


    }
}
