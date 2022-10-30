using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Finders
{
    public static class GoogleFinder
    {
        public static IWebElement? GetAcceptCookieButton(IWebDriver driver)
        {
            var elements = driver.FindElements(By.XPath("//*[@role='none']"));

            IWebElement? divWithRole = null;
            if (elements.Count == 0)
                return null;
            else if (elements.Count == 1)
                divWithRole = elements.First();
            else if (elements.Count > 1)
                divWithRole = elements.Last();

            return divWithRole?.FindElement(By.XPath("./.."));
        }

        public static IWebElement? GetMainGoogleSearchInput(IWebDriver driver)
        {
            var elements = driver.FindElements(By.XPath("//*[@type='text']"));
            foreach(var el in elements)
            {
                if(el.GetAttribute("name") == "q"
                    || el.GetAttribute("maxlength") == "2048"
                    || el.GetAttribute("autocorrect") == "off")
                {
                    return el;
                }
            }
            return null;
        }
    }
}
