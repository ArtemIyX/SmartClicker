using MaterialDesignColors;
using OpenQA.Selenium;
using SmartClicker_WPF.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Finders
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
                string aria_lable = link.GetAttribute("aria-label");
                if (!string.IsNullOrEmpty(aria_lable))
                {
                    //Let's split our aria-lable by space
                    string[] splited = aria_lable.Split(" ");
                    int pageNum;
                    //Second string will be our number
                    if (int.TryParse(splited[1], out pageNum))
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
            IWebElement div_rso = driver.FindElement(By.Id("rso"));
            //Let's find all links
            ReadOnlyCollection<IWebElement>? links = div_rso.FindElementsSave(By.TagName("a"));
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
            IWebElement? div_botstuff = driver.FindElementSave(By.Id("botstuff"));
            if (div_botstuff == null)
                return null;
            //If page is not loaded there are no tables
            ReadOnlyCollection<IWebElement>? tables = div_botstuff.FindElements(By.TagName("table"));
            if (tables == null)
                return null;
            if (tables.Count == 0)
                return null;
            //Table must be one
            IWebElement first_talbe = tables.First();
            //And this table has attribute role
            if (first_talbe.GetAttribute("role") == "presentation")
            {
                return first_talbe;
            }
            return null;
        }
    }
}
