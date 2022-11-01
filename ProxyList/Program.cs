
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ProxyList.ProxyTools;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net.Cache;
using System.Text;
using System.Text.Json.Nodes;

public class Program
{
    public static string DriverPath = "chromedriver_win32";
    public static int ProxyCallback = 5000;

    public static string[] proxies;
    public static List<string> working = new List<string>();
    public static string? url;
    public static int time = 0;
    public static int count = 0;

    public static async Task Main(string[] args)
    {
        Console.Write("Proxy list: ");
        string? filePath = Console.ReadLine();
       
        try
        {
            proxies = await File.ReadAllLinesAsync(filePath);
            Console.Write("Url: ");
            url = Console.ReadLine();
            Console.Write("Time in seconds: ");
            time = int.Parse(Console.ReadLine());
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
            return;
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Running...");
        Console.ResetColor();

        ProxyService proxyService = new ProxyService();

        for (int i = 0; i < proxies.Length; i++)
        {
            await DoWork(proxyService, proxies[i]);
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Done. Total: {count}/{proxies.Length}");
        Console.ResetColor();

        await SaveWorking();
        
        Console.ReadLine();
        //224
        //3
        //10
        //1
    }

    public static async Task SaveWorking()
    {
        TextWriter tw = new StreamWriter("WorkingProxies.txt");
        foreach (string str in working)
            await tw.WriteLineAsync(str);

        tw.Close();
    }

    public static async Task DoWork(ProxyService proxyService, string proxy)
    {
        bool check = await proxyService.CheckProxy(proxy, ProxyCallback);
        string sign = check ? "+" : "-";
        Console.ForegroundColor = check ? ConsoleColor.Green : ConsoleColor.DarkYellow;
        Console.WriteLine($"[{sign}]\t{proxy}\n");
        Console.ResetColor();
        if (check)
        {
            IWebDriver? drv = null;
            try
            {
                drv = GetChromeDriver(DriverPath, proxy);
                drv.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
                drv.Navigate().GoToUrl(url);
                await Task.Delay(time * 1000);
                count++;
                working.Add(proxy);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                if (drv != null)
                {
                    drv.Quit();
                    drv.Dispose();
                }
            }
        }
    }

    public static ChromeDriver GetChromeDriver(string driverPath, string proxy)
    {
        var p = new Proxy()
        {
            SslProxy = proxy,
            HttpProxy = proxy
        };
        var options = new ChromeOptions();
        options.Proxy = p;
        options.AddArguments("--disable-extensions");
        var driver = new ChromeDriver(driverPath, options, TimeSpan.FromSeconds(30));
        return driver;
    }
}
