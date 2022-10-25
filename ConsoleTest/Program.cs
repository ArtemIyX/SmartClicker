using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Data;
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
        baseUrl = @"https://www.google.com/";
        webDriver = GetChromeDriver();
        webDriver.Navigate().GoToUrl(baseUrl);
        Thread.Sleep(5000);
        var search_form = GetGoogleSearchMainInput();
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

    public static ChromeDriver GetChromeDriver()
    {
        var proxy = new Proxy();
        proxy.SocksProxy = "2.139.162.80:4145";
        proxy.SocksVersion = 4;
        var options = new ChromeOptions();
        options.Proxy = proxy;
        return new ChromeDriver(driverPath, options, TimeSpan.FromSeconds(60));
    }
}