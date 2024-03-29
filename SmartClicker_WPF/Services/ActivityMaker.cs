﻿using OpenQA.Selenium;
using SmartClicker_WPF.Extensions;
using SmartClicker_WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace SmartClicker_WPF.Services
{
    public class ActivityMaker
    {
        private readonly WebDriver _driver;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancellationToken;
        private static readonly Random MakerRandom;

        //wait until new page with another url is loaded
        public int LinkClickTimeoutS { get; set; } = 10;
        //Find ad banner
        public int FindLinkTimeoutS { get; set; } = 2;
        public int DelayBetweenActivityMs { get; set; } = 500;

        private int DelayMs => UseRandomDelay ? 
            MakerRandom.Next(DelayBetweenActivityMs / 5, DelayBetweenActivityMs + (DelayBetweenActivityMs / 2)) 
            : DelayBetweenActivityMs;

        public bool UseRandomDelay { get;set; }
        //Pop-up ad
        private string GoogleDismisButtonId { get; set; } = "dismiss-button";

        static ActivityMaker()
        {
            MakerRandom = new Random();
        }
        public ActivityMaker(WebDriver webDriver)
        {
            _driver = webDriver;

        }


        public async Task DoActivityFor(int seconds, CancellationToken cancellationToken)
        {
            _cancelTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancelTokenSource.Token;
            _cancelTokenSource.CancelAfter(seconds * 1000);
            while (true)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                cancellationToken.ThrowIfCancellationRequested();

                int n = MakerRandom.Next(4);
                for (int i = 0; i < n; ++i)
                {
                    await ScrollToRandomElement(cancellationToken);
                }

                bool result = await GoToRandomLink(cancellationToken);
                if (result)
                {
                    await Task.Delay(DelayMs);
                }
                else
                {
                    _driver.Navigate().GoToUrl("https://" + new Uri(_driver.Url).Host);
                }
            }
        }

        private (IWebElement? frame, IWebElement? buttonDiv) CheckForPopUpAd()
        {
            ReadOnlyCollection<IWebElement>? iframes = _driver.FindElementsSave(By.TagName("iframe"));

            if (iframes != null)
            {
                foreach (var f in iframes)
                {
                    _driver.SwitchTo().Frame(f);
                    IWebElement? dismisButton = _driver.FindElementSave(By.Id(GoogleDismisButtonId));
                    _driver.SwitchTo().DefaultContent();
                    if (dismisButton != null)
                    {
                        return (frame: f, buttonDiv: dismisButton);
                    }
                }
            }

            return (null, null);
        }

        private void TrySkipPopUp()
        {
            try
            {
                (IWebElement? frame, IWebElement? button) popUp = CheckForPopUpAd();
                if (popUp.frame != null && popUp.button != null)
                {
                    _driver.SwitchTo().Frame(popUp.frame);
                    popUp.button.ClickSave();
                    _driver.SwitchTo().DefaultContent();
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                _driver.SwitchTo().DefaultContent();
            }
        }


        private async Task<bool> GoToRandomLink(CancellationToken cancellationToken)
        {
            string currentUrl = _driver.Url;
            int i = 500;
            while (i > 0)
            {
                i--;

                TrySkipPopUp();
                
                // Pick random url
                IWebElement? el = await SiteFinder.GetRandomLinkAsync(_driver, _cancellationToken);

                // Web element is correct
                if (el == null) continue;

                //Link has href
                string href = el.GetAttribute("href");
                if (string.IsNullOrEmpty(href)) continue;

                //And this link contains our domain
                Uri myUri = new Uri(currentUrl);
                if (!href.Contains(myUri.Host)) continue;

                // Scroll to link
                if (!await _driver.ScrollTo(el))
                {
                    _driver.Navigate().Refresh();
                    return false;
                }

                // Click in link
                if (!el.ClickSave())
                {
                    _driver.Navigate().Refresh();
                    return false;
                }

                await Task.Delay(DelayMs, cancellationToken);
                // Find new element in site
                bool loadedNewPage = await WaitUntilNewPageIsLoaded(currentUrl);
                if (!loadedNewPage)
                    return false;

                // and now our url is different
                if (currentUrl != _driver.Url)
                    return true;
            }
            return false;
        }

        private async Task<bool> WaitUntilNewPageIsLoaded(string currentUrl)
        {
            IWebElement? newElement = await _driver.FindElementAsync(LinkClickTimeoutS, drv =>
            {
                // Another url 
                if (currentUrl == _driver.Url)
                {
                    return null;
                }
                else
                {
                    //And has div
                    return drv.FindElementSave(By.TagName("div"));
                }
            }, _cancellationToken);
            return (newElement != null);
        }

        private async Task<bool> ScrollToRandomElement(CancellationToken cancellationToken)
        {
            int i = 500;
            while (i > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                i--;
                IWebElement? el = GetRandomElement();
                if (el != null)
                {
                    return await _driver.ScrollTo(el);
                }
            }
            return false;
        }

        private IWebElement? GetRandomElement()
        {
            int select = MakerRandom.Next(1, 4);
            IWebElement? el = null;
            int i = 500;
            while (i > 0)
            {
                switch (select)
                {
                    case 1:
                        el = SiteFinder.GetRandomLink(_driver);
                        break;
                    case 2:
                        el = SiteFinder.GetRandomDiv(_driver);
                        break;
                    case 3:
                        el = SiteFinder.GetRandomP(_driver);
                        break;
                }
                if (el != null)
                {
                    return el;
                }
                i--;
            }
            return null;
        }

        private static bool CheckXpath(string xpath)
        {
            try
            {
                XPathExpression.Compile(xpath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static (bool suc, string? attr, string? value) CheckAttributeValue(string input)
        {
            Regex regex = new Regex("([^\"]*)=\\\"([^\"]*)\\\"");
            MatchCollection matches = regex.Matches(input);
            if (matches.Count > 0 && matches[0].Groups.Count > 1)
            {
                return (suc: true, attr: matches[0].Groups[0].Value, value: matches[0].Groups[1].Value);
            }
            return (suc: false, attr: null, value: null);
        }

        private static bool CheckIfAdIsCorrect(AdDetect adDetect)
        {
            if (string.IsNullOrEmpty(adDetect.Value))
            {
                return false;
            }

            switch (adDetect.Type)
            {
                case AdDetectType.AttributeValue:
                    (bool suc, string? attr, string? value) check = CheckAttributeValue(adDetect.Value);
                    return check.suc;
                case AdDetectType.XPath:
                    return CheckXpath(adDetect.Value);
                default:
                    return true;
            }
        }

        private static Func<IWebDriver, IWebElement?> GetAdCondition(AdDetect adDetect)
        {
            if (!CheckIfAdIsCorrect(adDetect))
            {
                throw new ArgumentException("Incorrect format", nameof(adDetect));
            }
            switch (adDetect.Type)
            {
                // Tag name (a/div/p)
                case AdDetectType.TagName:
                    return drv => drv.FindElementSave(By.TagName(adDetect.Value));
                // //*[@href]
                case AdDetectType.HasAttribute:
                    return drv => drv.FindElementSave(By.XPath($"//*[@{adDetect.Value}]"));
                // //*[@class="nav-links"]
                case AdDetectType.AttributeValue:
                    return drv => drv.FindElementSave(By.XPath($"//*[@{adDetect.Value}]"));
                // //*[@class="someClass"]
                case AdDetectType.HasClass:
                    return drv => drv.FindElementSave(By.XPath($"//*[@class={adDetect.Value}"));
                // TagName == a && href.contains(value)
                case AdDetectType.ContainsUrl:
                    return drv =>
                    {
                        if (string.IsNullOrEmpty(adDetect.Value))
                            return null;

                        ReadOnlyCollection<IWebElement>? elements = drv.FindElementsSave(By.TagName("a"));
                        if (elements == null)
                            return null;
                        List<IWebElement> resultElements = new List<IWebElement>();
                        foreach(var link in elements)
                        {
                            string href = link.GetAttribute("href");
                            if (!string.IsNullOrEmpty(href))
                            {
                                if (href.Contains(adDetect.Value))
                                {
                                    resultElements.Add(link);
                                }
                            }
                        }
                        if (resultElements.Count == 0)
                        {
                            return null;
                        }
                        return resultElements.RandomElement();
                    };
                // Custom CSS
                case AdDetectType.Css:
                    return drv => drv.FindElementSave(By.CssSelector(adDetect.Value));
                // Custom Xpath
                case AdDetectType.XPath:
                    return drv => drv.FindElementSave(By.TagName(adDetect.Value));
            }
            //Never call this
            return drv => drv.FindElementSave(By.TagName("a"));
        }

        private async Task<(IWebElement? iframe, IWebElement? link)> FindAd(AdDetect adDetect)
        {
            try
            {
                ReadOnlyCollection<IWebElement>? iframes = _driver.FindElementsSave(By.TagName("iframe"));
                if (iframes == null)
                    return (null, null);
                if (!iframes.Any())
                    return (null, null);

                int n = iframes.Count(); 
                while(n > 0)
                {
                    n--;
                    var iframe = iframes.RandomElement();
                    _driver.SwitchTo().Frame(iframe);
                    IWebElement? link = await _driver.FindElementAsync(FindLinkTimeoutS, GetAdCondition(adDetect), _cancellationToken);
                    _driver.SwitchTo().DefaultContent();
                    if (link != null)
                    {
                        return (iframe, link);
                    }
                }
            }
            catch
            {
                return (null, null);
            }
            return (null, null);
        }

        private async Task<(IWebElement? iframe, IWebElement? banner)> FindAd(ICollection<AdDetect> adDetects)
        {
            for (int i = 0; i < adDetects.Count; ++i)
            {
                (IWebElement? iframe, IWebElement? banner) result = await FindAd(adDetects.ElementAt(i));
                if (result.iframe != null && result.banner != null)
                {
                    return result;
                }
            }
            return (null, null);
        }

        public async Task<(IWebElement iframe, IWebElement banner)> FindAdBannerBy(
            int seconds, ICollection<AdDetect> adDetects)
        {
            if (adDetects == null)
                throw new ArgumentNullException(nameof(adDetects));
            if (adDetects.Count == 0)
                throw new ArgumentException ("No detects", nameof(adDetects));

            _cancelTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancelTokenSource.Token;
            _cancelTokenSource.CancelAfter(seconds * 1000); 

            while (true)
            {
                if (_cancellationToken.IsCancellationRequested)
                    _cancellationToken.ThrowIfCancellationRequested();

                TrySkipPopUp();

                (IWebElement? iframe, IWebElement? banner) result = await FindAd(adDetects);
                if (result.iframe != null && result.banner != null)
                {
                    return (result.iframe, result.banner);
                }
                await GoToRandomLink(_cancellationToken);
                await Task.Delay(DelayMs, _cancellationToken);
            }
        }
    }
}
