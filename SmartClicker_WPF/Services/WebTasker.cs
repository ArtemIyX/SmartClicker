using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SmartClicker_WPF.Extensions;
using SmartClicker_WPF.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Services
{
    public enum WebTaskerState
    {
        None,
        AcceptingGoogleCookie,
        TypingSearchRequest,
        PressingSearchingButton,
        SearchingForWebSite,
        GoingToSite,
        DoingActivityOnSite,
        SearchingForAdBanner,
        DoingActivityOnAdSite
    }

    public class WebTasker
    {
        private static readonly string GoogleUrl = @"https://www.google.com/";

        private WebDriver? _driver;
        private readonly int _loops;
        private readonly ProxyService? _proxyService;
        private readonly ICollection<string> _proxies;
        private readonly ICollection<AdDetect> _adDetects;
        private readonly WebProxyType _proxyType;
        private readonly WebDriverType _webDriverType;
        private string? _username;
        private string? _password;
        private readonly bool _useProxy;
        private readonly bool _checkProxy;
        private readonly WebService _webService;
        private readonly string _site;
        private readonly string _driverPath;
        private readonly int _timeOut;
        private readonly CancellationToken _cancellationToken;

        private Random _rand = new Random();
        private const int _minDelay = 250;
        private const int _maxDelay = 750;
        private int RandDelay() => _rand.Next(_minDelay, _maxDelay);

        private ActivityMaker AMaker => new(_driver)
        {
            DelayBetweenActivityMs = DelayBetweenActivity,
            FindLinkTimeoutS = TimeOutFindLink,
            LinkClickTimeoutS = TimeOutLinkClick,
            UseRandomDelay = RandomDelayBetweenActivity
        };

        private List<string> _keys;
        private int _pageIndex = 1;
        private int _proxyIndex = 0;

        public event Action<string> OnFinished;
        public event Action OnCompleted;
        public event Action<string> OnLog;
        public event Action<int> OnIterationChanged;
        public event Action<string> OnStatusChanged;
        public event Action<int> OnAdClicksChanged;
        public event Action<string> OnUsingProxy;

        public bool IsInProgress { get; private set; } = false;

        private int _currentIteration = 0;

        public int CurrentIteration
        {
            get => _currentIteration;
            private set
            {
                _currentIteration = value;
                OnIterationChanged.Invoke(_currentIteration);
            }
        }

        private WebTaskerState _state = WebTaskerState.None;

        public WebTaskerState State
        {
            get => _state;
            private set
            {
                _state = value;
                OnStatusChanged.Invoke(_state.ToString());
            }
        }

        private int _adClicks = 0;

        public int AdClicks
        {
            get => _adClicks;
            private set
            {
                _adClicks = value;
                OnAdClicksChanged.Invoke(_adClicks);
            }
        }

        //TODO: To struct

        //Find 'cookie button' on google.com
        public int FindCookieButtonTimeOutS { get; set; } = 10;

        //Find 'search bar' on google.com
        public int FindSearchBarTimeOutS { get; set; } = 10;

        //Find 'search button' on google.com
        public int FindSearchButtonTimeOutS { get; set; } = 10;

        //Find nav table in google.com/search?q=
        public int NavTableSearchTiemOutS { get; set; } = 10;

        //Find page in nav table
        public int PageSearchTimeOutS { get; set; } = 5;

        //Wait until page is loaded
        public int PageLoadTimeOutS { get; set; } = 60;

        //Wait until proxy send respond
        public int ProxyCheckTimeOutMs { get; set; } = 5000;

        //Max page to find site
        public int MaxPageCount { get; set; } = 10;
        public int TimeOutFindLink { get; set; }
        public int TimeOutLinkClick { get; set; }
        public int DelayBetweenActivity { get; set; }
        public bool RandomDelayBetweenActivity { get; set; }
        public bool HideConsole { get; set; }
        public bool HideBrowser { get; set; }

        public WebTasker(
            CancellationToken cancellationToken,
            WebService webService,
            InputService inputService,
            string site,
            string keywords,
            string driverPath,
            int timeOut,
            WebDriverType webDriverType,
            int loops,
            ICollection<AdDetect> adDetects)
        {
            this._cancellationToken = cancellationToken;
            _webService = webService;
            _site = site;
            _keys = inputService.SplitKeyWords(keywords);
            _driverPath = driverPath;
            _timeOut = timeOut;
            _webDriverType = webDriverType;
            _loops = loops;
            _adDetects = adDetects;
            _useProxy = false;
        }

        public WebTasker(
            CancellationToken cancellationToken,
            WebService webService,
            InputService inputService,
            string site,
            string keywords,
            string driverPath,
            int timeOut,
            WebDriverType webDriverType,
            int loops,
            ICollection<AdDetect> adDetects,
            ProxyService proxyService,
            bool checkProxy,
            ICollection<string> proxies,
            WebProxyType proxyType,
            string? username = null,
            string? password = null)
            : this(cancellationToken, webService, inputService, site, keywords, driverPath, timeOut, webDriverType,
                loops, adDetects)
        {
            _proxyService = proxyService;
            _proxies = proxies;
            _proxyType = proxyType;
            _username = username;
            _password = password;
            _useProxy = true;
            _checkProxy = checkProxy;
        }

        //TODO: Check why dont work
        // Main function
        public async Task StartWork()
        {
            try
            {
                if (IsInProgress)
                    throw new Exception("Already in progress");

                IsInProgress = true;
                _proxyIndex = 0;

                for (int i = 0; i < _loops; ++i)
                {
                    CurrentIteration = i;
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        Complete("Aborted");
                        return;
                    }

                    await CheckProxyBeforeUsage();
                    if (_useProxy)
                    {
                        OnUsingProxy.Invoke(_proxies.ElementAt(_proxyIndex));
                    }

                    await DoCycle();

                    if (_useProxy)
                    {
                        IncreaseProxyIndex();
                    }
                }

                Complete("Completed");
            }
            catch (Exception ex)
            {
                Complete(ex.Message);
            }
        }

        private void Complete(string reason)
        {
            FinishWork(reason + $" Clicks: {_adClicks}, Iterations: {_currentIteration}");
            IsInProgress = false;
            AdClicks = 0;
            CurrentIteration = 0;
            OnCompleted.Invoke();
        }

        private async Task CheckProxyBeforeUsage()
        {
            if (!_useProxy)
            {
                return;
            }

            if (!_checkProxy)
            {
                return;
            }

            while (true)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                string proxy = _proxies.ElementAt(_proxyIndex);

                bool working = false;
                OnLog.Invoke($"Checking proxy {proxy}...");
                if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                {
                    working = await _proxyService.CheckProxy(_cancellationToken, proxy, ProxyCheckTimeOutMs, _username,
                        _password);
                }
                else
                {
                    working = await _proxyService.CheckProxy(_cancellationToken, proxy, ProxyCheckTimeOutMs);
                }

                _cancellationToken.ThrowIfCancellationRequested();
                if (working)
                {
                    OnLog.Invoke($"Working with proxy {proxy}");
                    return;
                }
                else
                {
                    OnLog.Invoke($"Proxy {proxy} don't work");
                    IncreaseProxyIndex();
                }
            }
        }

        private async Task CheckSharedProxy()
        {
            if (_useProxy)
            {
                await Task.Delay(1000);
                if (_driver.Url.Contains("www.google.com/sorry/"))
                {
                    throw new Exception("There is a captcha");
                }
            }
        }

        private async Task DoCycle()
        {
            try
            {
                _cancellationToken.ThrowIfCancellationRequested();

                InitDriver(_proxyIndex);

                if (_driver == null)
                    throw new Exception("Driver has not been initialized");

                _cancellationToken.ThrowIfCancellationRequested();

                _driver.Navigate().GoToUrl(GoogleUrl);
                _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(PageLoadTimeOutS);

                await TryAcceptGoogleCookies();

                await TypeSearchingQeury(_keys.RandomElement());
                await PressSearchingButton();

                await CheckSharedProxy();

                IWebElement link = await FindWebsiteLink();
                await GoToSiteByLink(link);

                State = WebTaskerState.DoingActivityOnSite;
                await DoSomeActivityFor(_timeOut);

                if (await GoToAdSite(_timeOut))
                {
                    AdClicks = AdClicks + 1;
                    State = WebTaskerState.DoingActivityOnAdSite;
                    await DoSomeActivityFor(_timeOut / 2);
                }

                _cancellationToken.ThrowIfCancellationRequested();
                FinishWork("Succes");
            }
            catch (Exception ex)
            {
                FinishWork($"Error: {ex.Message}");
            }
        }

        private int IncreaseProxyIndex()
        {
            _proxyIndex = Math.Clamp(_proxyIndex + 1, 0, _proxies.Count);
            if (_proxyIndex == _proxies.Count)
            {
                _proxyIndex = 0;
            }

            return _proxyIndex;
        }

        public void FinishWork(string reason)
        {
            try
            {
                if (_driver != null)
                {
                    _driver.Manage().Cookies.DeleteAllCookies();
                    _driver.Quit();
                    _driver.Dispose();
                    _driver = null;
                }
            }
            catch
            {
            }
            finally
            {
                _pageIndex = 1;
                State = WebTaskerState.None;
                OnFinished.Invoke(reason);
            }
        }

        private async Task<bool> GoToAdSite(int seconds)
        {
            State = WebTaskerState.SearchingForAdBanner;

            OnLog.Invoke($"Searching for ad in {_driver.Url} for {seconds}s...");
            int i = 5;
            while (i > 0)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    ActivityMaker activityMaker = AMaker;
                    (IWebElement iframe, IWebElement link) result =
                        await activityMaker.FindAdBannerBy(seconds, _adDetects);

                    _cancellationToken.ThrowIfCancellationRequested();
                    OnLog.Invoke("Found ad banner");
                    _driver.SwitchTo().Frame(result.iframe);
                    if (!await _driver.ScrollTo(result.link))
                    {
                        OnLog.Invoke("Can not scroll to link, but trying to click");
                    }

                    _cancellationToken.ThrowIfCancellationRequested();

                    bool clicked = result.link.ClickSave();
                    _driver.SwitchTo().DefaultContent();
                    if (clicked)
                    {
                        OnLog.Invoke("Clicked on banner");
                        await Task.Delay(RandDelay());
                        return true;
                    }
                    else
                    {
                        OnLog.Invoke($"Can not click on link (ad banner) ({5 - i}/5)");
                    }
                }
                catch
                {
                    //OnLog.Invoke("");
                    throw new Exception("Time is up, did not find ad banner");
                }

                i--;
            }

            OnLog.Invoke("After 5 attempts couldn't click on banner");
            return false;
        }

        // Do activity on site
        private async Task DoSomeActivityFor(int seconds)
        {
            OnLog.Invoke($"Doing some activity in {_driver.Url} for {seconds}s...");
            ActivityMaker activityMaker = AMaker;
            try
            {
                await activityMaker.DoActivityFor(seconds, _cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                OnLog.Invoke("Activity is done");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Try find site by search result
        private async Task<IWebElement> FindWebsiteLink()
        {
            State = WebTaskerState.SearchingForWebSite;

            OnLog.Invoke($"Searching for website {_site}...");
            _pageIndex = 1;
            while (true)
            {
                _cancellationToken.ThrowIfCancellationRequested();

                var (s1, nav_table) = await GetNavTable();
                if (!s1)
                {
                    throw new Exception("Can not find google nav-bar");
                }

                // Find link on tihs page
                IWebElement? link = GoogleFinder.FindUrlInSearch(_driver, _site);
                // If no link - go to next page
                if (link == null)
                {
                    if (_pageIndex > MaxPageCount)
                    {
                        throw new Exception($"We are already on the {_pageIndex} page and did not find {_site}");
                    }

                    if (!await OpenNextPage(nav_table))
                    {
                        throw new Exception(($"Can not find google nav-bar (for page{_pageIndex}) link"));
                    }
                }
                else
                {
                    return link;
                }
            }
        }

        // Get navigation table of google (bottom numbers)
        private async Task<(bool, IWebElement?)> GetNavTable()
        {
            OnLog.Invoke($"Waiting for page {_pageIndex}...");

            // Wait until page is loaded
            IWebElement? nav_table = await _driver.FindElementAsync(NavTableSearchTiemOutS,
                drv => GoogleFinder.GetGooglePageTable(drv), _cancellationToken);
            _cancellationToken.ThrowIfCancellationRequested();
            if (nav_table == null)
            {
                return (false, null);
            }


            await Task.Delay(RandDelay());
            OnLog.Invoke($"Page {_pageIndex} is loaded");
            return (true, nav_table);
        }

        //Scroll to link and click
        private async Task GoToSiteByLink(IWebElement link)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            OnLog.Invoke($"Url found on page {_pageIndex}");

            State = WebTaskerState.GoingToSite;

            await _driver.ScrollTo(link, _minDelay, _maxDelay);
            if (!link.ClickSave())
            {
                throw new Exception("Can not click on site url");
            }

            _cancellationToken.ThrowIfCancellationRequested();

            OnLog.Invoke("Going to site..");
            await Task.Delay(RandDelay());
        }

        // Try go to next page 
        private async Task<bool> OpenNextPage(IWebElement nav_table)
        {
            _pageIndex++;

            IWebElement? pageLink = await _driver.FindElementAsync(PageSearchTimeOutS,
                drv => GoogleFinder.GetGooglePageLink(nav_table, _pageIndex));

            if (pageLink == null)
            {
                return false;
            }

            if (await _driver.ScrollTo(pageLink, _minDelay, _maxDelay))
            {
                return pageLink.ClickSave();
            }

            return true;
        }

        // Main page - press "Find in google"
        private async Task PressSearchingButton()
        {
            State = WebTaskerState.PressingSearchingButton;

            OnLog.Invoke("Looking for google search button...");

            IWebElement? searchButton = await _driver.FindElementAsync(FindSearchButtonTimeOutS,
                drv => GoogleFinder.GetMainGoogleSearchButton(drv), _cancellationToken);
            _cancellationToken.ThrowIfCancellationRequested();
            if (searchButton == null)
            {
                throw new Exception("Can not find google search button");
            }

            _cancellationToken.ThrowIfCancellationRequested();
            OnLog.Invoke("Found google search button");
            await Task.Delay(RandDelay());
            if (!searchButton.ClickSave())
            {
                throw new Exception("Can not click on google button");
            }

            _cancellationToken.ThrowIfCancellationRequested();
            OnLog.Invoke("Clicked on google search button");
        }

        // Main page - type some query
        private async Task TypeSearchingQeury(string query)
        {
            State = WebTaskerState.TypingSearchRequest;

            OnLog.Invoke("Looking for google search input...");
            IWebElement? searchInput = await _driver.FindElementAsync(FindSearchBarTimeOutS,
                drv => GoogleFinder.GetMainGoogleSearchInput(drv), _cancellationToken);
            _cancellationToken.ThrowIfCancellationRequested();
            if (searchInput == null)
            {
                throw new Exception("Can not find google search input");
            }


            OnLog.Invoke("Found google search input");

            if (!searchInput.SendKeysSave(query))
            {
                throw new Exception("Can not send keys to google input");
            }

            if (!_driver.ClickOnBlankArea())
            {
                throw new Exception("Can not click on blank area");
            }

            await Task.Delay(RandDelay());
        }

        // Accept google cookies
        private async Task TryAcceptGoogleCookies()
        {
            State = WebTaskerState.AcceptingGoogleCookie;

            OnLog.Invoke("Looking for cookies button...");
            IWebElement? cookieButton = await _driver.FindElementAsync(FindCookieButtonTimeOutS,
                drv => GoogleFinder.GetAcceptCookieButton(drv), _cancellationToken);

            _cancellationToken.ThrowIfCancellationRequested();

            if (cookieButton != null)
            {
                await Task.Delay(RandDelay());
                cookieButton.ClickSave();

                _cancellationToken.ThrowIfCancellationRequested();
                OnLog.Invoke("Accepted cookies");
            }
            else
            {
                _cancellationToken.ThrowIfCancellationRequested();
                OnLog.Invoke("Cookies button not found");
            }
        }

        public void InitDriver(int proxyIndex = 0)
        {
            if (_useProxy)
            {
                if (!string.IsNullOrEmpty(_username) &&
                    !string.IsNullOrEmpty(_password))
                {
                    _driver = _webService.CreateWebDriverWithPrivateProxy(_driverPath,
                        _webDriverType,
                        _proxyType,
                        _proxies.ElementAt(proxyIndex),
                        _username,
                        _password,
                        HideBrowser,
                        HideConsole);
                }
                else
                {
                    _driver = _webService.CreateWebDriverWithProxy(_driverPath,
                        _webDriverType,
                        _proxyType,
                        _proxies.ElementAt(proxyIndex),
                        HideBrowser,
                        HideConsole);
                }
            }
            else
            {
                _driver = _webService.CreateWebDriver(_driverPath, _webDriverType, HideBrowser,
                    HideConsole);
            }
        }
    }
}