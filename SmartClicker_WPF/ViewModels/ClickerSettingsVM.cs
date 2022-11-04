using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenQA.Selenium.DevTools.V104.Emulation;
using SmartClicker_WPF.Interfaces;
using SmartClicker_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartClicker_WPF.Services;

namespace SmartClicker_WPF.ViewModels
{
    [ObservableObject]
    public partial class ClickerSettingsVM : ISettingsVM
    {
        private readonly SettingsService _settingsService;
        public ClickerSettingsVM(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        public void InsertSettings(SettingsJson settingsJson)
        {
            TimeOut_CookieButton = settingsJson.TimeOut_CookieButton.ToString();
            TimeOut_SearchBar = settingsJson.TimeOut_SearchBar.ToString();
            TimeOut_SearchButton = settingsJson.TimeOut_SearchButton.ToString();
            TimeOut_SearchSite = settingsJson.TimeOut_SearchSite.ToString();
            TimeOut_NavTable = settingsJson.TimeOut_NavTable.ToString();
            TimeOut_PageLoad = settingsJson.TimeOut_PageLoad.ToString();
            TimeOut_Page = settingsJson.TimeOut_Page.ToString();
            TimeOut_ProxyCheckerMs = settingsJson.TimeOut_ProxyCheckerMs.ToString();
            TimeOut_FindLink = settingsJson.TimeOut_FindLink.ToString();
            TimeOut_LinkClick = settingsJson.TimeOut_LinkClick.ToString();
            MaxPageCount = settingsJson.MaxPageCount.ToString();
            DelayBetweenActivityMs = settingsJson.DelayBetweenActivityMs.ToString();
            HideConsole = settingsJson.HideConsole;
            HideBrowser = settingsJson.HideBrowser;
            RandomDelayBetweenActivity = settingsJson.RandomDelayBetweenActivity;
        }

        public void ModifySettings(SettingsJson settingsJson)
        {
            settingsJson.TimeOut_CookieButton = GetValueSave(TimeOut_CookieButton, settingsJson.TimeOut_CookieButton);
            settingsJson.TimeOut_SearchBar = GetValueSave(TimeOut_SearchBar, settingsJson.TimeOut_SearchBar);
            settingsJson.TimeOut_SearchButton = GetValueSave(TimeOut_SearchButton, settingsJson.TimeOut_SearchButton);
            settingsJson.TimeOut_SearchSite = GetValueSave(TimeOut_SearchSite, settingsJson.TimeOut_SearchSite);
            settingsJson.TimeOut_NavTable = GetValueSave(TimeOut_NavTable, settingsJson.TimeOut_NavTable);
            settingsJson.TimeOut_PageLoad = GetValueSave(TimeOut_PageLoad, settingsJson.TimeOut_PageLoad);
            settingsJson.TimeOut_Page = GetValueSave(TimeOut_Page, settingsJson.TimeOut_Page);
            settingsJson.TimeOut_ProxyCheckerMs = GetValueSave(TimeOut_ProxyCheckerMs, settingsJson.TimeOut_ProxyCheckerMs);
            settingsJson.TimeOut_FindLink = GetValueSave(TimeOut_FindLink, settingsJson.TimeOut_FindLink);
            settingsJson.TimeOut_LinkClick = GetValueSave(TimeOut_LinkClick, settingsJson.TimeOut_LinkClick);
            settingsJson.MaxPageCount = GetValueSave(MaxPageCount, settingsJson.MaxPageCount);
            settingsJson.DelayBetweenActivityMs = GetValueSave(DelayBetweenActivityMs, settingsJson.DelayBetweenActivityMs);
            settingsJson.HideConsole = _hideConsole;
            settingsJson.HideBrowser = _hideBrowser;
            settingsJson.RandomDelayBetweenActivity = RandomDelayBetweenActivity;
        }

        private int GetValueSave(string variable, int defaultValue)
        {
            try
            {
                return int.Parse(variable);
            }
            catch
            {
                return defaultValue;

            }
        }

        [RelayCommand]
        public void Reset() => InsertSettings(_settingsService.LoadDefaultSettingsObject());

        [ObservableProperty]
        private string _timeOut_CookieButton;
        [ObservableProperty]
        private string _timeOut_SearchBar;
        [ObservableProperty]
        private string _timeOut_SearchButton;
        [ObservableProperty]
        private string _timeOut_SearchSite;
        [ObservableProperty]
        private string _timeOut_NavTable;
        [ObservableProperty]
        private string _timeOut_Page;
        [ObservableProperty]
        private string _timeOut_PageLoad;
        [ObservableProperty]
        private string _timeOut_FindLink;
        [ObservableProperty]
        private string _timeOut_LinkClick;

        [ObservableProperty]
        private string _maxPageCount;


        [ObservableProperty]
        private string _timeOut_ProxyCheckerMs;
        [ObservableProperty]
        private string _delayBetweenActivityMs;

        [ObservableProperty]
        private bool _hideConsole;
        [ObservableProperty]
        private bool _hideBrowser;
        [ObservableProperty]
        private bool _randomDelayBetweenActivity;


    }
}
