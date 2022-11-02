using MaterialDesignThemes.Wpf;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chrome.ChromeDriverExtensions;
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
        baseUrl = "https://about.ip2c.org";
        Console.Write("IP: ");
        //string ip = Console.ReadLine();
        webDriver = GetChromeDriver();
        webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
        webDriver.Navigate().GoToUrl(baseUrl);

    }


    //public static Driver

    public static ChromeDriver GetChromeDriver()
    {
        var proxy = new Proxy();
        //proxy.HttpProxy = "168.181.229.173:50100";
        //proxy.SslProxy = "168.181.229.173:50100";
        //proxy.SocksPassword = "79uBp6hDoW";
        var options = new ChromeOptions();
        options.AddHttpProxy("168.181.229.173", 50100, "evgeniypo", "79uBp6hDoW");
        //ptions.Proxy = proxy;
        //options.AddArguments("--disable-extensions");
        var driver = new ChromeDriver(driverPath, options, TimeSpan.FromSeconds(120));
        return driver;
    }
}