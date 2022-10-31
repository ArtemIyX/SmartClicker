using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Support.UI;
using SmartClicker_WPF.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Services
{
    public class ActivityMaker
    {
        private readonly WebDriver _driver;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancellationToken;
        public static Random MakerRandom;

        public int LinkTimeout { get; set; } = 10;
        public string GoogleDismisButtonId { get; set; } = "dismiss-button";
        public string GoogleAdDivId { get; set; } = "ad_position_box";

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

                await Task.Delay(500);
            }
        }

        private async Task<bool> GoToRandomLink()
        {
            string currentUrl = _driver.Url;
            int i = 500;
            while (i > 0)
            {
                i--;
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
            IWebElement? newElement = await _driver.FindElementAsync(LinkTimeout, drv =>
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
    }
}
