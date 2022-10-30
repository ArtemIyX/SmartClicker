using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static async Task SlowScrollTo(IWebDriver webDriver, string cssSelector, int speed = 100, int minDelay = 250, int maxDelay = 1000)
        {
            await Task.Delay(MakerRandom.Next(minDelay, maxDelay));
            IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
            double y = (double)js.ExecuteScript($"return document.querySelector('{cssSelector}').getBoundingClientRect()['y']");
            Console.WriteLine(y);
            for (int i = 0; i < (int)y; i += speed)
            {
                js.ExecuteScript("window.scrollTo(0, " + i +");");
            }
            await Task.Delay(MakerRandom.Next(minDelay, maxDelay));
        }
        public static async Task SlowScrollTo(IWebDriver webDriver, IWebElement webElement, int speed = 100, int minDelay = 250, int maxDelay = 1000)
        {
            await Task.Delay(MakerRandom.Next(minDelay, maxDelay));
            IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
            ILocatable locatable = (ILocatable)webElement;
            ICoordinates viewPortLocation = locatable.Coordinates;
            int y = viewPortLocation.LocationInViewport.Y;
            for (int i = 0; i < y; i += speed)
            {
                js.ExecuteScript("window.scrollTo(0, " + i + ");");
            }
            await Task.Delay(MakerRandom.Next(minDelay, maxDelay) * 2);
        }

        public static async Task ScrollTo(IWebDriver webDriver, IWebElement webElement, int minDelay = 250, int maxDelay = 1000)
        {
            await Task.Delay(MakerRandom.Next(minDelay, maxDelay));
            Actions actions = new Actions(webDriver);
            actions.MoveToElement(webElement);
            actions.Perform();
            await Task.Delay(MakerRandom.Next(minDelay, maxDelay));
        }
      

        public static Task<IWebElement?> WaitUntilElementFound(IWebDriver webDriver, int waitTimeOut, Func<IWebDriver, IWebElement?> Condition)
            => Task.Run(() =>
            {
                try
                {
                    return new WebDriverWait(webDriver, new TimeSpan(0, 0, waitTimeOut)).Until(Condition);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            });

        public static async Task<(IWebElement?, Exception? ex)> WaitUntilElementFoundSave(IWebDriver webDriver, int waitTimeOut, Func<IWebDriver, IWebElement?> Condition)
        {
            try
            {
                IWebElement? el = await WaitUntilElementFound(webDriver, waitTimeOut, Condition);
                return (el, null);
            }
            catch (Exception ex)
            {
                return (null, ex);
            }
        }
        private async Task GoToRandomLink()
        {

            string currentUrl = _driver.Url;
            int i = 500;
            while (i > 0)
            {
                // Pick random url
                IWebElement? el = SiteFinder.GetRandomLink(_driver);

                // Web element is correct
                if (el != null)
                {
                    //Link has href
                    string href = el.GetAttribute("href");
                    if (!string.IsNullOrEmpty(href))
                    {
                        //And this link contains our domain
                        Uri myUri = new Uri(currentUrl);
                        if (href.Contains(myUri.Host))
                        {
                            // Scroll to link
                            try
                            {
                                await ActivityMaker.SlowScrollTo(_driver, el);
                            }
                            catch (Exception)
                            {
                                _driver.Navigate().Refresh();
                                return;
                            }

                            // Click in link
                            try
                            {
                                el.Click();
                            }
                            catch (Exception)
                            {
                                _driver.Navigate().Refresh();
                                return;
                            }
                            // Wait
                            await Task.Delay(1000);
                            // Find new element in site
                            (IWebElement? newElement, Exception? ex) = await ActivityMaker.WaitUntilElementFoundSave(_driver, LinkTimeout, drv =>
                            {
                                return drv.FindElement(By.TagName("div"));
                            });
                            // we have element
                            if (newElement != null)
                            {
                                // and now our url is different
                                if (currentUrl != _driver.Url)
                                {
                                    return;
                                }
                            }
                        }
                    }

                }
                i--;
            }
        }



        private async Task ScrollToRandomElement()
        {
            int i = 500;
            while (i > 0)
            {
                IWebElement? el = GetRandomElement();
                if (el != null)
                {
                    try
                    {
                        await ActivityMaker.SlowScrollTo(_driver, el);
                        return;
                    }
                    catch (Exception)
                    {

                    }
                }
                i--;
            }
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
