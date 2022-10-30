using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
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
        webDriver.Navigate().GoToUrl(baseUrl);
        /*
         y = driver.execute_script("return document.querySelector('YOUR-CSS-SELECTOR').getBoundingClientRect()['y']")
        for x in range(0, int(y), 100):
            driver.execute_script("window.scrollTo(0, "+str(x)+");")
         */
        Thread.Sleep(2500);
        WebElement el = (WebElement)webDriver.FindElement(By.ClassName("tagcloud"));
        ILocatable locatable = (ILocatable)el;
        ICoordinates viewPortLocation = locatable.Coordinates;
        int x = viewPortLocation.LocationInViewport.X;
        int y = viewPortLocation.LocationInViewport.Y;
        Console.WriteLine($"C#: x:{x}, y:{y}");
        IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
        double _y = (double)js.ExecuteScript("return document.querySelector('.tagcloud').getBoundingClientRect()['y']");
        Console.WriteLine($"JS: {_y}");

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
        var driver = new ChromeDriver(driverPath, options, TimeSpan.FromSeconds(30));
        return driver;
    }
}