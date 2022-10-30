
using OpenQA.Selenium;
using SmartClicker_WPF.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Services
{
    public static class SiteFinder
    {
        public static IWebElement? GetRandomLink(IWebDriver webDriver)
        {
            try
            {
                return webDriver.FindElements(By.TagName("a")).PickRandom();
            }
            catch(Exception)
            {
                return null;
            }
        }

        public static IWebElement? GetRandomDiv(IWebDriver webDriver)
        {
            try
            {
                return webDriver.FindElements(By.TagName("div")).PickRandom();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IWebElement? GetRandomP(IWebDriver webDriver)
        {
            try
            {
                return webDriver.FindElements(By.TagName("p")).PickRandom();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
