using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
            Waiter waiter = new Waiter(webDriver, timeOutS, condition);
            return await waiter.Wait();
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
    }
}
