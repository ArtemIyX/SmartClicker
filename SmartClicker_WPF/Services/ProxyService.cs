using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Services
{
    public enum ProxyServiceType
    {
        Http,
        Socks4,
        Socks5
    }
    public class ProxyService
    {
        public ProxyService()
        {
            
        }
        public ICollection<ProxyServiceType> GetProxyTypes() 
            => Enum.GetValues<ProxyServiceType>();

        public List<string> GetProxyTypesString() =>
            Enum.GetValues<ProxyServiceType>().Select(x => x.ToString()).ToList();

        public Task<List<string>> GetProxyTypesStringAsync() =>
            Task.Run(() => {
                return Enum.GetValues<ProxyServiceType>().Select(x => x.ToString()).ToList();
            });

    }
}
