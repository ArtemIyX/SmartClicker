using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SmartClicker_WPF.Interfaces;
using SmartClicker_WPF.Models;
using SmartClicker_WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.ViewModels
{
    [ObservableObject]
    public partial class DriverSettingsVM : ISettingsVM
    {
        private readonly SettingsService _settingsService;
        public DriverSettingsVM(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [ObservableProperty]
        private string? _chromePath;

        [ObservableProperty]
        private string? _firefoxPath;

        [ObservableProperty]
        private string? _edgePath;


        public void InsertSettings(SettingsJson settingsJson)
        {
            ChromePath = settingsJson.ChromeDriverPath;
            FirefoxPath = settingsJson.FirefoxDriverPath;
        }

        public void ModifySettings(SettingsJson settingsJson)
        {
            settingsJson.ChromeDriverPath = ChromePath;
            settingsJson.FirefoxDriverPath = FirefoxPath;
        }

        private (bool, string) SelectLocation()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Driver (*.exe)|*.exe|All files (*.*)|*.*";
            bool? res = openFileDialog.ShowDialog();
            if (res == null)
                throw new Exception("Unknown path");
            if (res == false)
                return (false, "");
            return (true, openFileDialog.FileName);
        }

        [RelayCommand]
        public void ResetBtn() => InsertSettings(_settingsService.LoadDefaultSettingsObject());

        [RelayCommand]
        public void ChromeSelect()
        {
            var (success, content) = SelectLocation();
            if(success)
            {
                ChromePath = content;
            }
        }
        [RelayCommand]
        public void FirefoxSelect()
        {
            var (success, content) = SelectLocation();
            if (success)
            {
                FirefoxPath = content;
            }
        }
        [RelayCommand]
        public void EdgeSelect()
        {
            var (success, content) = SelectLocation();
            if (success)
            {
                EdgePath = content;
            }
        }

    }
}
