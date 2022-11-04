using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using SmartClicker_WPF.Extensions;

namespace SmartClicker_WPF.Services
{
    public static class GoogleFinder
    {
        public static IWebElement? GetAcceptCookieButton(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement>? elements = driver.FindElementsSave(By.XPath("//*[@role='none']"));
            
            IWebElement? divWithRole = null;
            if (elements == null)
                return null;
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
            ReadOnlyCollection<IWebElement>? elements = driver.FindElementsSave(By.XPath("//*[@type='text']"));
            if (elements == null)
                return null;
            foreach (var el in elements)
            {
                if (el.GetAttribute("name") == "q"
                    || el.GetAttribute("maxlength") == "2048"
                    || el.GetAttribute("autocorrect") == "off")
                {
                    return el;
                }
            }
            return null;

        }
        public static IWebElement? GetMainGoogleSearchButton(IWebDriver driver)
        {
            //Search button have name 'btnK'
            ReadOnlyCollection<IWebElement>? elements = driver.FindElementsSave(By.Name("btnK"));
            if (elements == null)
                return null;
            if (elements.Count == 0)
                return null;
            return elements.Last();
        }

        public static IWebElement? GetGooglePageLink(IWebElement table, int page)
        {
            //All links in table
            ReadOnlyCollection<IWebElement>? links = table.FindElementsSave(By.TagName("a"));
            if (links == null) return null;
            foreach (var link in links)
            {
                //if(link.GetAttribute("class") == "f1")
                //{
                //In attribute 'aria-lable' we have "Page N"
                string href = link.GetAttribute("href");
                string ariaLable = link.GetAttribute("aria-label");
                if (!string.IsNullOrEmpty(ariaLable))
                {
                    //Let's split our aria-lable by space
                    string[] splited = ariaLable.Split(" ");
                    //Second string will be our number
                    if (int.TryParse(splited[1], out int pageNum))
                    {
                        if (pageNum == page)
                        {
                            return link;
                        }
                    }
                }
                //}
            }
            return null;
        }


        public static IWebElement? FindUrlInSearch(IWebDriver driver, string url)
        {
            //Div with id 'rso' is div with results of searching
            IWebElement divRso = driver.FindElement(By.Id("rso"));
            //Let's find all links
            ReadOnlyCollection<IWebElement>? links = divRso.FindElementsSave(By.TagName("a"));
            if (links == null) return null;
            foreach (var a in links)
            {
                string href = a.GetAttribute("href");
                if (!string.IsNullOrEmpty(href))
                {
                    //Href must contain our url
                    if (href.Contains(url))
                    {
                        return a;
                    }
                }
            }
            return null;
        }

        public static IWebElement? GetGooglePageTable(IWebDriver driver)
        {
            //Our table is located in div with id 'dotstuff'
            IWebElement? divBotstuff = driver.FindElementSave(By.Id("botstuff"));
            if (divBotstuff == null)
                return null;
            //If page is not loaded there are no tables
            ReadOnlyCollection<IWebElement>? tables = divBotstuff.FindElements(By.TagName("table"));
            if (tables == null)
                return null;
            if (tables.Count == 0)
                return null;
            //Table must be one
            IWebElement firstTalbe = tables.First();
            //And this table has attribute role
            if (firstTalbe.GetAttribute("role") == "presentation")
            {
                return firstTalbe;
            }
            return null;
        }
    }
}
