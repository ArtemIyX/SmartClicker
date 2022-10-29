
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Cache;
using System.Text;
using System.Text.Json.Nodes;
using static System.Net.WebRequestMethods;

public class Program
{
    public enum ProtocolType
    {
        Https,
        Socks4,
        Socks5
    }
    public static string CallUrl = @"https://proxylist.geonode.com/api/proxy-list?";

    public static async Task Main(string[] args)
    {
        try
        {
            int limit = GetInt("Limit of proxy per page: ", 1, 1000);
            int page = GetInt("Page: ", 1, 50);
            int type = GetInt($"Select type:\n{GetProtocolSelectionPrompt()}", 1, Enum.GetNames<ProtocolType>().Length);
            string path = GetString("Filename to save proxies: ");
            string jsonRaw = await SendRequest(limit, page, (ProtocolType)(type - 1));
            await DownloadProxies(jsonRaw, path);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        Console.Read();
    }
    public static async Task DownloadProxies(string jsonRaw, string path)
    {
        var t = Task.Run(() =>
        {
            WriteToFileProxies(path, GetDataFromRawJson(jsonRaw));
            return true;
        });
        while(!t.IsCompleted)
        {
            Console.Write(".");
            await Task.Delay(1);
        }
        Console.WriteLine("\nCompleted!");
    }

    public static async Task<string> SendRequest(int limit, int page, ProtocolType type)
    {
        HttpClient client = new HttpClient();
        string query = QueryString(limit, page, (ProtocolType)(type - 1));
        var result = await client.GetStringAsync(query);
        return result;
    }

    public static void WriteToFileProxies(string path, JArray data)
    {
        using (StreamWriter w = System.IO.File.AppendText(path))
        {
            for (int i = 0; i < data.Count; ++i)
            {
                JObject proxy = (JObject)data[i];
                w.WriteLine((string)proxy["ip"]);
            }
        }
    }

    public static JArray GetDataFromRawJson(string jsonRaw)
    {
        JObject json = (JObject)JsonConvert.DeserializeObject(jsonRaw)!;
        JArray data = (JArray)json["data"]!;
        return data;
    }

    public static string GetProtocolSelectionPrompt()
    {
        List<string> types = Enum.GetValues<ProtocolType>().Select(x => x.ToString()).ToList();
        StringBuilder sbStr = new StringBuilder();
        for (int i = 0; i < types.Count; i++)
        {
            sbStr.Append($"{i + 1}.{types[i]}");
            if (i < types.Count - 1)
                sbStr.Append("\n");
        }
        return sbStr.ToString();

    }
    public static string GetString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    public static int GetInt(string prompt, int min = 0, int max = 1000)
    {
        Console.Write(prompt + "\n-> ");
        int res = int.Parse(Console.ReadLine());
        return Math.Clamp(res, min, max);
    }

    public static string QueryString(int limit, int page, ProtocolType type)
    {
        //return @"https://proxylist.geonode.com/api/proxy-list?limit=500&page=1&sort_by=lastChecked&sort_type=desc&protocols=socks4";
        return $"{CallUrl}limit={limit}&page={page}&sort_by=lastChecked&sort_type=desc&protocols={type.ToString().ToLower()}";
    }
}
