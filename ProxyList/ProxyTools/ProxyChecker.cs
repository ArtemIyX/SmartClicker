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
    public class ProxyService
    {
        private static string ProxyCheckApiUrl = @"http://ip-api.com/json/?fields=61439";

        private WebProxy CreateWebProxy(string ip, string port)
        {
            return new WebProxy(ip, int.Parse(port));
        }

        private HttpClientHandler CreateProxyHandler(string ip, string port)
        {
            return new HttpClientHandler()
            {
                Proxy = CreateWebProxy(ip, port)
            };
        }

        private HttpClientHandler CreateProxyHandler(string ip, string port,
            string username, string password)
        {
            return new HttpClientHandler()
            {
                Proxy = CreateWebProxy(ip, port),
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };
        }
        private async Task<bool> TrySendRequest(HttpClientHandler clientHandler, int timeOutMs)
        {
            using (var httpClient = new HttpClient(handler: clientHandler, disposeHandler: true)
            {
                Timeout = TimeSpan.FromMilliseconds(timeOutMs)
            })
            {
                try
                {
                    var response = await httpClient.GetAsync(ProxyCheckApiUrl);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public async Task<bool> CheckProxy(string proxy, int timeOutMs)
        {
            var port_ip = proxy.Split(":");
            var httpClientHandler = CreateProxyHandler(port_ip[0], port_ip[1]);
            return await TrySendRequest(httpClientHandler, timeOutMs);

        }

        public async Task<bool> CheckProxy(string proxy, int timeOutMs, string username, string password)
        {
            var port_ip = proxy.Split(":");
            var httpClientHandler = CreateProxyHandler(port_ip[0], port_ip[1], username, password);
            return await TrySendRequest(httpClientHandler, timeOutMs);

        }
    }
}
