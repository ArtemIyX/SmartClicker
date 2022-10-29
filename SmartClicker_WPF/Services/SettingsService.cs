using Newtonsoft.Json;
using SmartClicker_WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Services
{
    public class SettingsService
    {
        public static string SavePath = "settings.json";

        public SettingsJson LoadDefaultSettingsObject()
            => new SettingsJson()
            {
                EdgeDriverPath = AppDomain.CurrentDomain.BaseDirectory + "edgedriver_win64",
                FirefoxDriverPath = AppDomain.CurrentDomain.BaseDirectory + "geckodriver-v0.32.0-win32",
                ChromeDriverPath = AppDomain.CurrentDomain.BaseDirectory + "chromedriver_win32"
            };
        private string FullSavePath => AppDomain.CurrentDomain.BaseDirectory + SavePath;

        public SettingsJson GetSettingsObject()
        {
            string content = "";
            SettingsJson? settings = null;

            if (File.Exists(FullSavePath))
            {
                content = File.ReadAllText(FullSavePath);
                settings =
                    JsonConvert.DeserializeObject<SettingsJson>(content);
                if (settings == null)
                    throw new Exception("Can not load settings.json");
                return settings;
            }
            else
            {
                settings = LoadDefaultSettingsObject();
                content = JsonConvert.SerializeObject(settings);
                File.WriteAllText(FullSavePath, content);
            }
            return settings;
        }

        public async Task<SettingsJson?> GetSettingsObjectAsync()
        {
            string fullPath = AppDomain.CurrentDomain.BaseDirectory + SavePath;
            string content = "";
            SettingsJson? settings = null;

            if (File.Exists(fullPath))
            {
                content = await File.ReadAllTextAsync(fullPath);
                settings =
                    JsonConvert.DeserializeObject<SettingsJson>(content);
                if (settings == null)
                    throw new Exception("Can not load settings.json");
                return settings;
            }
            else
            {
                settings = LoadDefaultSettingsObject();
                content = JsonConvert.SerializeObject(settings);
                await File.WriteAllTextAsync(fullPath, content);
            }
            return settings;
        }

        public void SaveSettingsObject(SettingsJson settingsJson)
        {
            string content = JsonConvert.SerializeObject(settingsJson);
            File.WriteAllText(FullSavePath, content);
        }

        public async Task SaveSettingsObjectAsync(SettingsJson settingsJson)
        {
            string content = JsonConvert.SerializeObject(settingsJson);
            await File.WriteAllTextAsync(FullSavePath, content);
        }

        public ICollection<Driver> GetDrivers(SettingsJson settingsJson)
        {
            List<Driver> result = new List<Driver>();
            result.Add(new Driver() { Path = settingsJson.ChromeDriverPath, Title = WebDriverType.Chrome.ToString() });
            result.Add(new Driver() { Path = settingsJson.FirefoxDriverPath, Title = WebDriverType.Firefox.ToString() });
            //result.Add(new Driver() { Path = settingsJson.EdgeDriverPath, Title = WebDriverType.Edge.ToString() });
            return result;
        }
    }
}
