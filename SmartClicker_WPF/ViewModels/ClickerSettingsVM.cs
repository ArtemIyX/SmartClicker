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

namespace SmartClicker_WPF.ViewModels
{
    [ObservableObject]
    public partial class ClickerSettingsVM : ISettingsVM
    {
        public ClickerSettingsVM()
        {

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
            DelayBettwenActivityMs = settingsJson.DelayBetweenActivityMs.ToString();
            GoogleDismisButtonId = settingsJson.GoogleDismisButtonId ?? "";
            HideConsole = settingsJson.HideConsole;
            HideBrowser = settingsJson.HideBrowser;
            RandomDelayBetweenActivity = settingsJson.RandomDelayBetweenActivity;
        }

        public void ModifySettings(SettingsJson settingsJson)
        {
            settingsJson.TimeOut_CookieButton = GetValueSave(_timeOut_CookieButton, settingsJson.TimeOut_CookieButton);
            settingsJson.TimeOut_SearchBar = GetValueSave(_timeOut_SearchBar, settingsJson.TimeOut_SearchBar);
            settingsJson.TimeOut_SearchButton = GetValueSave(_timeOut_SearchButton, settingsJson.TimeOut_SearchButton);
            settingsJson.TimeOut_SearchSite = GetValueSave(_timeOut_SearchSite, settingsJson.TimeOut_SearchSite);
            settingsJson.TimeOut_NavTable = GetValueSave(_timeOut_NavTable, settingsJson.TimeOut_NavTable);
            settingsJson.TimeOut_PageLoad = GetValueSave(_timeOut_PageLoad, settingsJson.TimeOut_PageLoad);
            settingsJson.TimeOut_Page = GetValueSave(_timeOut_Page, settingsJson.TimeOut_Page);
            settingsJson.TimeOut_ProxyCheckerMs = GetValueSave(_timeOut_ProxyCheckerMs, settingsJson.TimeOut_ProxyCheckerMs);
            settingsJson.TimeOut_FindLink = GetValueSave(_timeOut_FindLink, settingsJson.TimeOut_FindLink);
            settingsJson.TimeOut_LinkClick = GetValueSave(_timeOut_LinkClick, settingsJson.TimeOut_LinkClick);
            settingsJson.MaxPageCount = GetValueSave(_maxPageCount, settingsJson.MaxPageCount);
            settingsJson.DelayBetweenActivityMs = GetValueSave(_delayBettwenActivityMs, settingsJson.DelayBetweenActivityMs);
            settingsJson.GoogleDismisButtonId = _googleDismisButtonId;
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
        public void Reset()
        {

        }

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
        private string _timeOut_ProxyCheckerMs;
        [ObservableProperty]
        private string _maxPageCount;
        [ObservableProperty]
        private string _timeOut_FindLink;
        [ObservableProperty]
        private string _timeOut_LinkClick;
        [ObservableProperty]
        private string _delayBettwenActivityMs;
        [ObservableProperty]
        private string _googleDismisButtonId;

        [ObservableProperty]
        private bool _hideConsole;
        [ObservableProperty]
        private bool _hideBrowser;
        [ObservableProperty]
        private bool _randomDelayBetweenActivity;

    }
}
