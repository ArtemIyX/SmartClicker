using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SmartClicker_WPF.Extensions;
using SmartClicker_WPF.Finders;
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
        private static string GoogleURL = @"https://www.google.com/";

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
        private int _proxyIndex = 0;

        public event Action<string> OnFinished;
        public event Action OnCompleted;
        public event Action<string> OnLog;

        public bool IsInProgress { get; private set; } = false;
        public int CurrentIteration { get; private set; } = 0;
        public WebTaskerState State { get; private set; } = WebTaskerState.None;

        //TODO: To struct

        //Find 'cookie button' on google.com
        public int FindCookieButtonTimeOutS { get; set; } = 10;
        //Find 'search bar' on google.com
        public int FindSearchBarTimeOutS { get; set; } = 10;
        //Find 'search button' on google.com
        public int FindSearchButtonTimeOutS { get; set; } = 10;
        //Find site in Google search results
        public int WebSiteSearchTimeOutS { get; set; } = 10;
        //Find nav table in google.com/serach?q=
        public int NavTableSearchTiemOutS { get; set; } = 10;
        //Find page in nav table
        public int PageSearchTimeOutS { get; set; } = 5;
        //Wait until page is loaded
        public int PageLoadTimeOutS { get; set; } = 60;
        //Wait unti proxy send respond
        public int ProxyCheckTimeOutMs { get; set; } = 5000;
        //Max page to find site
        public int MaxPageCount { get; set; } = 10;


        public WebTasker(
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
            _webService = webService;
            _inputService = inputService;
            _site = site;
            _keywords = keywords;
            _keys = _inputService.SplitKeyWords(_keywords);
            _driverPath = driverPath;
            _timeOut = timeOut;
            _webDriverType = webDriverType;
            _loops = loops;
            _adDetects = adDetects;
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
            ICollection<AdDetect> adDetects,
            ProxyService proxyService,
            ICollection<string> proxies,
            WebProxyType proxyType,
            string? username = null,
            string? password = null)
            : this(webService, inputService, site, keywords, driverPath, timeOut, webDriverType, loops, adDetects)
        {
            _proxyService = proxyService;
            _proxies = proxies;
            _proxyType = proxyType;
            _username = username;
            _password = password;
            _useProxy = true;
        }

        //TODO: Check why dont work
        // Main function
        public async Task StartWork(CancellationToken cancellationToken)
        {
            if (IsInProgress)
                throw new Exception("Already in progress");

            _proxyIndex = 0;

            for (int i = 0; i < _loops; ++i)
            {
                CurrentIteration = i;
                if (cancellationToken.IsCancellationRequested)
                {
                    Complete("Aborted");
                    return;
                }

                await CheckProxyBeforeUsage(cancellationToken);
                await DoCycle(cancellationToken);

                if (_useProxy)
                {
                    IncreaseProxyIndex();
                }
            }

            Complete("Completed");
        }
        private void Complete(string reason)
        {
            FinishWork(reason);
            IsInProgress = false;
            OnCompleted.Invoke();
        }
        private async Task CheckProxyBeforeUsage(CancellationToken cancellationToken)
        {
            if (!_useProxy)
            {
                return;
            }
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string proxy = _proxies.ElementAt(_proxyIndex);

                bool working = false;
                if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                {
                    working = await _proxyService.CheckProxy(proxy, ProxyCheckTimeOutMs, _username, _password);
                }
                else
                {
                    working = await _proxyService.CheckProxy(proxy, ProxyCheckTimeOutMs);
                }
                cancellationToken.ThrowIfCancellationRequested();
                if (working)
                {
                    OnLog.Invoke($"Working with proxy {proxy}");
                    return;
                }
                else
                {
                    OnLog.Invoke($"Proxy {proxy} don't work");
                    IncreaseProxyIndex();
                   /* if ( == 0)
                    {
                        throw new Exception("No working proxy");
                    }*/
                }
            }

        }

        private async Task DoCycle(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                InitDriver(_proxyIndex);

                if (_driver == null)
                    throw new Exception("Driver has not been initialized");

                IsInProgress = true;

                cancellationToken.ThrowIfCancellationRequested();

                _driver.Navigate().GoToUrl(GoogleURL);
                _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(PageLoadTimeOutS);

                await TryAcceptGoogleCookies(cancellationToken);

                await TypeSearchingQeury(_keys.RandomElement(), cancellationToken);
                await PressSearchingButton(cancellationToken);

                IWebElement link = await FindWebsiteLink(cancellationToken);
                await GoToSiteByLink(link, cancellationToken);

                State = WebTaskerState.DoingActivityOnSite;
                await DoSomeActivityFor(_timeOut, cancellationToken);

                if (await GoToAdSite(_timeOut, cancellationToken))
                {
                    State = WebTaskerState.DoingActivityOnAdSite;
                    await DoSomeActivityFor(_timeOut / 2, cancellationToken);
                }

                cancellationToken.ThrowIfCancellationRequested();
                FinishWork("Succes");
            }
            catch (Exception ex)
            {
                FinishWork($"Error: {ex.Message}");
            }
        }

        private int IncreaseProxyIndex()
        {
            _proxyIndex++;
            if(_proxyIndex == _proxies.Count)
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
                State = WebTaskerState.None;
                OnFinished.Invoke(reason);
            }
        }


        private async Task<bool> GoToAdSite(int seconds, CancellationToken cancellationToken)
        {
            State = WebTaskerState.SearchingForAdBanner;

            OnLog.Invoke($"Searching for ad in {_driver.Url} for {seconds}s...");
            int i = 5;
            while (i > 0)
            {
                try
                {
                    ActivityMaker activityMaker = new ActivityMaker(_driver);
                    (IWebElement iframe, IWebElement link) result =
                        await activityMaker.FindAdBannerBy(seconds, _adDetects);

                    OnLog.Invoke("Found ad banner");
                    _driver.SwitchTo().Frame(result.iframe);
                    if (!await _driver.ScrollTo(result.link))
                    {
                        OnLog.Invoke("Can not scroll to link, but trying to click");
                    }

                    bool clicked = result.link.ClickSave();
                    _driver.SwitchTo().DefaultContent();
                    if (clicked)
                    {
                        OnLog.Invoke("Clicked on banner");
                        await Task.Delay(randDelay());
                        return true;
                    }
                    else
                    {
                        OnLog.Invoke($"Can not click on link (ad banner) ({5 - i}/5)");
                    }
                }
                catch
                {
                    OnLog.Invoke("Time is up, did not find ad banner");
                }

                i--;
            }

            OnLog.Invoke("After 5 attempts couldn't click on banner");
            return false;
        }

        // Do activity on site
        private async Task DoSomeActivityFor(int seconds, CancellationToken cancellationToken)
        {
            OnLog.Invoke($"Doing some activity in {_driver.Url} for {seconds}s...");
            ActivityMaker activityMaker = new ActivityMaker(_driver);
            try
            {
                await activityMaker.DoActivityFor(seconds, cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                OnLog.Invoke("Activity is done");
            }
        }

        // Try find site by search result
        private async Task<IWebElement> FindWebsiteLink(CancellationToken cancellationToken)
        {
            State = WebTaskerState.SearchingForWebSite;

            OnLog.Invoke($"Searching for website {_site}...");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var (s1, nav_table) = await GetNavTable(cancellationToken);
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
        private async Task<(bool, IWebElement?)> GetNavTable(CancellationToken cancellationToken)
        {
            OnLog.Invoke($"Waiting for page {_pageIndex}...");

            // Wait until page is loaded
            IWebElement? nav_table = await _driver.FindElementAsync(NavTableSearchTiemOutS,
                drv => GoogleFinder.GetGooglePageTable(drv), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (nav_table == null)
            {
                return (false, null);
            }


            await Task.Delay(randDelay());
            OnLog.Invoke($"Page {_pageIndex} is loaded");
            return (true, nav_table);
        }

        //Scroll to link and click
        private async Task GoToSiteByLink(IWebElement link, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            OnLog.Invoke($"Url found on page {_pageIndex}");


            await _driver.ScrollTo(link, _minDelay, _maxDelay);
            if (!link.ClickSave())
            {
                throw new Exception("Can not click on site url");
            }

            cancellationToken.ThrowIfCancellationRequested();

            OnLog.Invoke("Going to site..");
            await Task.Delay(randDelay());
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
        private async Task PressSearchingButton(CancellationToken cancellationToken)
        {
            State = WebTaskerState.PressingSearchingButton;

            OnLog.Invoke("Looking for google search button...");

            IWebElement? searchButton = await _driver.FindElementAsync(FindSearchButtonTimeOutS,
                drv => GoogleFinder.GetMainGoogleSearchButton(drv), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (searchButton == null)
            {
                throw new Exception("Can not find google search button");
            }

            cancellationToken.ThrowIfCancellationRequested();
            OnLog.Invoke("Found google search button");
            await Task.Delay(randDelay());
            if (!searchButton.ClickSave())
            {
                throw new Exception("Can not click on google button");
            }

            cancellationToken.ThrowIfCancellationRequested();
            OnLog.Invoke("Clicked on google search button");
        }

        // Main page - type some query
        private async Task TypeSearchingQeury(string query, CancellationToken cancellationToken)
        {
            State = WebTaskerState.TypingSearchRequest;

            OnLog.Invoke("Looking for google search input...");
            IWebElement? searchInput = await _driver.FindElementAsync(FindSearchBarTimeOutS,
                drv => GoogleFinder.GetMainGoogleSearchInput(drv), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
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
            await Task.Delay(randDelay());
        }

        // Accept google cookies
        private async Task TryAcceptGoogleCookies(CancellationToken cancellationToken)
        {
            State = WebTaskerState.AcceptingGoogleCookie;

            OnLog.Invoke("Looking for cookies button...");
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, FindCookieButtonTimeOutS));
            IWebElement? cookieButton = await _driver.FindElementAsync(FindCookieButtonTimeOutS,
                drv => GoogleFinder.GetAcceptCookieButton(drv), cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            if (cookieButton != null)
            {
                await Task.Delay(randDelay());
                cookieButton.Click();

                cancellationToken.ThrowIfCancellationRequested();
                OnLog.Invoke("Accepted cookies");
            }
            else
            {
                cancellationToken.ThrowIfCancellationRequested();
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
                        _password);
                }
                else
                {
                    _driver = _webService.CreateWebDriverWithProxy(_driverPath,
                        _webDriverType,
                        _proxyType,
                        _proxies.ElementAt(proxyIndex));
                }
            }
            else
            {
                _driver = _webService.CreateWebDriver(_driverPath, _webDriverType);
            }
        }


    }
}
