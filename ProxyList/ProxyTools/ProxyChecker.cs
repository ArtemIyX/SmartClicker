using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ProxyList.ProxyTools
{
    public class ProxyChecker
    {
        public List<string> Proxies { get; set; } = new List<string>();
        public List<string> WorkingProxies { get; set; } = new List<string>();
        public List<string> NonWorkingProxies { get; set; } = new List<string>();

        public Task? CheckingTask { get; private set; }
        public int TotalChecked { get; private set; } = 0;

        public event Action<string?, string?> OnCheckedWorking;
        public event Action<string?, string?> OnCheckedDead;

        public ProxyChecker()
        {

        }
        public ProxyChecker(List<string> ProxiesToCheck)
        {
            Proxies = ProxiesToCheck;
        }

        public Task Check()
        {
            if(Proxies.Count == 0)
            {
                throw new Exception("No proxies to check");
            }
            WorkingProxies.Clear();
            NonWorkingProxies.Clear();


            CheckingTask = Task.Run(() =>
            {
                for (int i = 0; i < Proxies.Count; i++)
                {
                    string proxy = Proxies[i];

                    TotalChecked = 0;
                    string[] splitted = proxy.Split(':');
                    string ip = splitted[0];
                    string port = splitted[1];
                    Ping ping = new Ping();
                    PingReply reply = ping.Send(ip);

                    JObject jobject = JObject.Parse(new WebClient().DownloadString("http://ip-api.com/json/" + ip));
                    string? country = (string?)jobject["regionName"];

                    if (reply.Status == IPStatus.Success)
                    {
                        WorkingProxies.Add(proxy);
                        OnCheckedWorking.Invoke(proxy, country);
                    }
                    else
                    {   
                        NonWorkingProxies.Add(proxy);
                        OnCheckedDead.Invoke(proxy, country);
                    }
                    TotalChecked++;
                }
            });
            return CheckingTask;
        }
    }
}
