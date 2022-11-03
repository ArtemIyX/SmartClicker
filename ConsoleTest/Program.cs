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
        Console.Clear();
        Console.WriteLine(webDriver.PageSource);
        Thread.Sleep(2000);
        webDriver.Navigate().GoToUrl("https://www.google.com/");
        Console.Clear();

    }


    //public static Driver

    public static ChromeDriver GetChromeDriver()
    {
        var proxy = new Proxy();
        var chromeDriverService = ChromeDriverService.CreateDefaultService(driverPath);
        chromeDriverService.HideCommandPromptWindow = true;
        var options = new ChromeOptions();

        options.AddArguments(new List<string>() {
            "--silent-launch",
            "--no-startup-window",
            "no-sandbox",
            "headless",});

        var driver = new ChromeDriver(chromeDriverService, options, TimeSpan.FromSeconds(60));
        return driver;
    }
}