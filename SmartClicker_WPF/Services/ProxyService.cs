﻿using SmartClicker_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Services
{

    public class ProxyService
    {
        private static readonly string ProxyCheckApiUrl = @"http://ip-api.com/json/?fields=61439";
        public ICollection<WebProxyType> GetProxyTypes()
            => Enum.GetValues<WebProxyType>();

        public List<string> GetProxyTypesString() =>
            Enum.GetValues<WebProxyType>().Select(x => x.ToString()).ToList();

        public Task<List<string>> GetProxyTypesStringAsync() =>
            Task.Run(() =>
            {
                return Enum.GetValues<WebProxyType>().Select(x => x.ToString()).ToList();
            });

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
        private async Task<bool> TrySendRequest(HttpClientHandler clientHandler, int timeOutMs, CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient(handler: clientHandler, disposeHandler: true)
            {
                Timeout = TimeSpan.FromMilliseconds(timeOutMs)
            };
            try
            {
                /*HttpResponseMessage response =*/ await httpClient.GetAsync(ProxyCheckApiUrl, cancellationToken);
                return true;
            }
            catch(TaskCanceledException)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckProxy(CancellationToken cancellationToken, string proxy, int timeOutMs)
        {
            string[] portIp = proxy.Split(":");
            HttpClientHandler httpClientHandler = CreateProxyHandler(portIp[0], portIp[1]);
            return await TrySendRequest(httpClientHandler, timeOutMs, cancellationToken);
            
        }

        public async Task<bool> CheckProxy(CancellationToken cancellationToken, string proxy, int timeOutMs, string username, string password)
        {
            string[] portIp = proxy.Split(":");
            HttpClientHandler httpClientHandler = CreateProxyHandler(portIp[0], portIp[1], username, password);
            return await TrySendRequest(httpClientHandler, timeOutMs, cancellationToken);

        }

    }
}
