using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Extensions
{
    public static class WebDriverExtensions
    {
        public static IWebElement? FindElementSave(this IWebDriver webDriver, By by)
        {
            try
            {
                return webDriver.FindElement(by);
            }
            catch
            {
                return null;
            }
        }

        public static ReadOnlyCollection<IWebElement>? FindElementsSave(this IWebDriver webDriver, By by)
        {
            try
            {
                return webDriver.FindElements(by);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<IWebElement?> FindElementAsync(this IWebDriver webDriver, int timeOutS, Func<IWebDriver, IWebElement?> condition)
        {
            return await new Waiter(webDriver, timeOutS, condition).Wait(CancellationToken.None);
        }

        public static async Task<IWebElement?> FindElementAsync(this IWebDriver webDriver, int timeOutS, Func<IWebDriver, IWebElement?> condition,
            CancellationToken cancellationToken)
        {
            return await new Waiter(webDriver, timeOutS, condition).Wait(cancellationToken);
        }

        public static async Task<bool> ScrollToSlow(this IWebDriver webDriver, IWebElement webElement, int speed = 100, int minDelay = 250, int maxDelay = 1000)
        {
            try
            {
                Random rand = new Random();
                await Task.Delay(rand.Next(minDelay, maxDelay));

                IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
                ILocatable locatable = (ILocatable)webElement;
                ICoordinates viewPortLocation = locatable.Coordinates;
                int y = viewPortLocation.LocationInViewport.Y;

                for (int i = 0; i < y; i += speed)
                {
                    js.ExecuteScript("window.scrollTo(0, " + i + ");");
                }

                await Task.Delay(rand.Next(minDelay, maxDelay) * 2);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static async Task<bool> ScrollTo(this IWebDriver webDriver, IWebElement webElement, int minDelay = 250, int maxDelay = 1000)
        {
            try
            {
                Random rand = new Random();

                await Task.Delay(rand.Next(minDelay, maxDelay));

                Actions actions = new Actions(webDriver);
                actions.MoveToElement(webElement);
                actions.Perform();

                await Task.Delay(rand.Next(minDelay, maxDelay));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IWebElement? FindElementSave(this IWebElement webElement, By by)
        {
            try
            {
                return webElement.FindElement(by);
            }
            catch
            {
                return null;
            }
        }

        public static ReadOnlyCollection<IWebElement>? FindElementsSave(this IWebElement webElement, By by)
        {
            try
            {
                return webElement.FindElements(by);
            }
            catch
            {
                return null;
            }
        }

        public static bool ClickSave(this IWebElement webElement)
        {
            try
            {
                webElement.Click();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendKeysSave(this IWebElement webElement, string text)
        {
            try
            {
                webElement.Clear();
                webElement.SendKeys(text);
                return true;
            }
            catch
            {
                return false;
            }

        }
        public static bool ClickOnBlankArea(this IWebDriver drv)
        {
            try
            {
                new Actions(drv).MoveByOffset(0, 0).Click().Build().Perform();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}