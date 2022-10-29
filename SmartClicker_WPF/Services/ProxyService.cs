using OpenQA.Selenium;
using SmartClicker_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Services
{

    public class ProxyService
    {
        public ICollection<WebProxyType> GetProxyTypes() 
            => Enum.GetValues<WebProxyType>();

        public List<string> GetProxyTypesString() =>
            Enum.GetValues<WebProxyType>().Select(x => x.ToString()).ToList();

        public Task<List<string>> GetProxyTypesStringAsync() =>
            Task.Run(() => {
                return Enum.GetValues<WebProxyType>().Select(x => x.ToString()).ToList();
            });

    }
}
