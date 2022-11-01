using MaterialDesignThemes.Wpf;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Program
{
    private static WebDriver webDriver { get; set; } = null!;
    private static string driverPath { get; set; }
    private static string baseUrl { get; set; }
    public static void Main(string[] args)
    {
        driverPath = AppDomain.CurrentDomain.BaseDirectory + "chromedriver_win32";
        baseUrl = @"https://101gardentools.com/";
        webDriver = GetChromeDriver();
        webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
        webDriver.Navigate().GoToUrl(baseUrl);

        Thread.Sleep(2500);
        //File.WriteAllText("PageSource.html", webDriver.PageSource);
        var iframes = webDriver.FindElements(By.TagName("iframe"));
        foreach (var iframe in iframes)
        {
            webDriver.SwitchTo().DefaultContent();
            Console.WriteLine(iframe.TagName);
            webDriver.SwitchTo().Frame(iframe);
            var children = webDriver.FindElements(By.TagName("a"));
            foreach (var child in children)
            {
                string href = child.GetAttribute("href");
                if (!string.IsNullOrEmpty(href))
                {
                    Console.WriteLine("\t" + child.TagName + "href\t" + new Uri(href).Host);
                    if (href.Contains("adclick"))
                    {

                        child.Click();
                        webDriver.SwitchTo().DefaultContent();
                        Thread.Sleep(5000);

                    }
                }
            }
            /*
            foreach (var child in children)
            {
                string tagName = child.TagName;
                Console.WriteLine("\t" + tagName);
                
            }*/

        }
        /* ReadOnlyCollection<IWebElement> links = webDriver.FindElements(By.TagName("a"));
         foreach(var el in links)
         {
             var href = el.GetAttribute("href");
             Console.WriteLine($"a href=\t{href ?? "null"}");
         }
         Console.Read();*/
    }

    public static IWebElement GetGoogleSearchMainInput()
    {
        var forms = webDriver.FindElements(By.TagName("form"));
        foreach (var form in forms)
        {
            try
            {
                var attr = form.GetAttribute("role");
                if (attr == "search")
                {

                    Console.WriteLine("Found search form!");
                    return form;
                }
            }
            catch (Exception exc)
            {

            }
        }
        Console.WriteLine("Not found");
        return null;
    }

    //public static Driver

    public static EdgeDriver GetEdgeDriver()
    {
        var options = new EdgeOptions();
        string ip = "219.78.228.211:80";
        options.Proxy = new Proxy()
        {
            HttpProxy = ip,
            //FtpProxy = ip,
            SslProxy = ip,
            Kind = ProxyKind.Manual
        };
        //options.AddArgument("--proxy-server=219.78.228.211:80");
        options.AddArgument("ignore-certificate-errors");
        var driver = new EdgeDriver(driverPath, options, TimeSpan.FromSeconds(60));
        return driver;
    }

    public static ChromeDriver GetChromeDriver()
    {
        var proxy = new Proxy();
        var options = new ChromeOptions();
        options.AddArguments("--disable-extensions");
        var driver = new ChromeDriver(driverPath, options, TimeSpan.FromSeconds(30));
        return driver;
    }
}