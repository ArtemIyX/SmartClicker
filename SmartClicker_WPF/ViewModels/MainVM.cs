using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClicker_WPF.Models;
using SmartClicker_WPF.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.ViewModels
{
    [ObservableObject]
    public partial class MainVM
    {
        private SettingsService _settingsService;
        private SettingsJson _settingsJson;

        public MainVM(SettingsService SettingsService)
        {
            _settingsService = SettingsService;
            _settingsJson = _settingsService.GetSettingsObject();
        }

        public string? Test => _settingsJson.ChromeDriverPath;
    }
}
