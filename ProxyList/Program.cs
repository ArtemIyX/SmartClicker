
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProxyList.ProxyTools;
using System;
using System.Diagnostics;
using System.Net.Cache;
using System.Text;
using System.Text.Json.Nodes;

public class Program
{
    public enum ProtocolType
    {
        Https,
        Socks4,
        Socks5
    }
    private static ProxyChecker _proxyChecker;
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Enter path to proxies list: ");
        string? path = Console.ReadLine();
        List<string> proxies = File.ReadAllLines(path).ToList();
        Console.WriteLine("Checking...");
        _proxyChecker = new ProxyChecker(proxies);
        _proxyChecker.OnCheckedDead += ProxyChecker_OnCheckedDead;
        _proxyChecker.OnCheckedWorking += ProxyChecker_OnCheckedWorking;
        Console.WriteLine($"Loaded: {proxies.Count}");
        await _proxyChecker.Check();
        Console.Title = $"Cheeky Proxy Checker | Loaded: {_proxyChecker.Proxies.Count} proxies | Working proxies: {_proxyChecker.WorkingProxies.Count} | Dead proxies: {_proxyChecker.NonWorkingProxies.Count}";
        Console.ReadLine();
    }

    private static void ProxyChecker_OnCheckedWorking(string? proxy, string? country)
    {
        Console.WriteLine($" [+] {country} | {proxy}");
    }

    private static void ProxyChecker_OnCheckedDead(string? proxy, string? country)
    {
        Console.WriteLine($" [-] {country} | {proxy} | No response");
    }
}
