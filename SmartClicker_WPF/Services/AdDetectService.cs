using SmartClicker_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Services
{
    public class AdDetectService
    {
        public AdDetectService()
        {

        }

        public ICollection<AdDetectType> GetAddDetectTypes() => Enum.GetValues<AdDetectType>();

        public List<string> GetStringAddDetectTypes() => GetAddDetectTypes().Select(x => x.ToString()).ToList();

        public async Task<List<string>> GetStringAddDetectTypesAsync() 
            => await Task.Run(() => { return GetAddDetectTypes().Select(x => x.ToString()).ToList(); });
    }
}
