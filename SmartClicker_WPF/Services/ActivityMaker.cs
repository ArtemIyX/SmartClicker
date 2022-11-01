using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Support.UI;
using SmartClicker_WPF.Extensions;
using SmartClicker_WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Printing;
using System.Reflection.Metadata;
using System.Text;
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
        public static Random MakerRandom;

        public int LinkTimeouts { get; set; } = 10;
        public int AdTimeoutS { get; set; } = 10;
        public int DelayBettwenActivityMs { get; set; } = 500;
        //Pop-up ad
        public string GoogleDismisButtonId { get; set; } = "dismiss-button";

        static ActivityMaker()
        {
            MakerRandom = new Random();
        }
        public ActivityMaker(WebDriver webDriver)
        {
            _driver = webDriver;

        }


        public async Task DoActivityFor(int seconds)
        {
            _cancelTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancelTokenSource.Token;
            _cancelTokenSource.CancelAfter(seconds * 1000);
            while (true)
            {
                if (_cancellationToken.IsCancellationRequested)
                    _cancellationToken.ThrowIfCancellationRequested();

                int n = MakerRandom.Next(4);
                for (int i = 0; i < n; ++i)
                {
                    await ScrollToRandomElement();
                }

                await GoToRandomLink();

                await Task.Delay(DelayBettwenActivityMs);
            }
        }

        private (IWebElement? frame, IWebElement? buttonDiv) CheckForPopUpAd()
        {
            ReadOnlyCollection<IWebElement>? iframes = _driver.FindElementsSave(By.TagName("iframe"));
            foreach (var f in iframes)
            {
                _driver.SwitchTo().Frame(f);
                IWebElement? dismisButton = _driver.FindElementSave(By.Id(GoogleDismisButtonId));
                _driver.SwitchTo().DefaultContent();
                if(dismisButton != null)
                {
                    return (frame: f, buttonDiv: dismisButton);
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

            }
            finally
            {
                _driver.SwitchTo().DefaultContent();
            }
        }


        private async Task<bool> GoToRandomLink()
        {
            string currentUrl = _driver.Url;
            int i = 500;
            while (i > 0)
            {
                i--;

                TrySkipPopUp();

                // Pick random url
                IWebElement? el = SiteFinder.GetRandomLink(_driver);

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

                // Wait one second
                await Task.Delay(1000);
                // Find new element in site
                bool loadedNewPage = await WaitUntilNewPageIsLoaded(currentUrl);
                if (!loadedNewPage)
                {
                    return false;
                }

                // and now our url is different
                if (currentUrl != _driver.Url)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> WaitUntilNewPageIsLoaded(string currentUrl)
        {
            IWebElement? newElement = await _driver.FindElementAsync(LinkTimeouts, drv =>
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
            });
            return (newElement != null);
        }

        private async Task<bool> ScrollToRandomElement()
        {
            int i = 500;
            while (i > 0)
            {
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

            return true;
        }

        private static Func<IWebDriver, IWebElement?> GetAdCondition(AdDetect adDetect)
        {
            if (!CheckIfAdIsCorrect(adDetect))
            {
                throw new ArgumentException("adDetect", "Incorrect format");
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

                        foreach(var link in elements)
                        {
                            string href = link.GetAttribute("href");
                            if (!string.IsNullOrEmpty(href))
                            {
                                if (href.Contains(adDetect.Value))
                                {
                                    return link;
                                }
                            }
                        }
                        return null;
                    };
                // Custom CSS
                case AdDetectType.CSS:
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
                if (iframes.Count() == 0)
                    return (null, null);
                foreach(var iframe in iframes)
                {
                    _driver.SwitchTo().Frame(iframe);
                    IWebElement? link = await _driver.FindElementAsync(AdTimeoutS, GetAdCondition(adDetect));
                    _driver.SwitchTo().DefaultContent();
                    if(link != null)
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
                throw new ArgumentNullException("adDetects");
            if (adDetects.Count == 0)
                throw new ArgumentException("adDetets", "No detects");

            _cancelTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancelTokenSource.Token;
            _cancelTokenSource.CancelAfter(seconds * 1000); ;

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
                await GoToRandomLink();
                await Task.Delay(DelayBettwenActivityMs);
            }
        }
    }
}
