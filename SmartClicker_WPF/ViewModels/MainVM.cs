using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClicker_WPF.Models;
using SmartClicker_WPF.Services;
using SmartClicker_WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmartClicker_WPF.ViewModels
{
    [ObservableObject]
    public partial class MainVM
    {
        private SettingsService _settingsService;
        private SettingsJson _settingsJson;
        private FooManager _fooManager;
        private ProxyService _proxyService;
        public MainVM(SettingsService SettingsService, FooManager FooManager, ProxyService ProxyService)
        {
            _settingsService = SettingsService;
            _fooManager = FooManager;
            _proxyService = ProxyService;
            _settingsJson = _settingsService.GetSettingsObject();
            Drivers = new ObservableCollection<Driver>(_settingsService.GetDrivers(_settingsJson));
            Detects = new ObservableCollection<AdDetect>();
            ProxyTypes = new ObservableCollection<string>(_proxyService.GetProxyTypesString());
            SelectedProxyTypeIndex = 0;
            SelectedDriver = Drivers[0];
            TimeOut = 60;
            Loops = 5;
            SiteUrl = @"101gardentools.com";
        }

        [ObservableProperty]
        private bool _useProxy;

        [ObservableProperty]
        private string _proxyList;

        [ObservableProperty]
        private ObservableCollection<string> _proxyTypes;

        [ObservableProperty]
        private int _selectedProxyTypeIndex;

        [ObservableProperty]
        private string _proxyUserName;

        [ObservableProperty]
        private string _proxyPassword;

        [ObservableProperty]
        private int _timeOut;

        [ObservableProperty]
        private string _siteUrl;

        [ObservableProperty]
        private int _loops;

        [ObservableProperty]
        private ObservableCollection<Driver> _drivers;

        [ObservableProperty]
        private Driver _selectedDriver;

        [ObservableProperty]
        private ObservableCollection<AdDetect> _detects;

        [ObservableProperty]
        private AdDetect _selectedDetect;

        [RelayCommand]
        public void RemoveDetect(object sender)
        {
            if (Detects.Count <= 0)
                throw new Exception("No list box items (Ad detects)");

            AdDetect? selected = sender as AdDetect;
            if (selected == null)
                throw new Exception("Command sender is not AdDetect");

            Detects.Remove(selected);


        }

        [RelayCommand]
        public void EditDetect(object sender)
        {
            AdDetect? adDetect = sender as AdDetect;
            if (adDetect == null)
                throw new Exception("Command sender is not AdDetect");

            NewDetectWindow? window = _fooManager.ServiceProvider.GetService(typeof(NewDetectWindow)) as NewDetectWindow;
            if (window == null)
                throw new Exception("Can not get service (NewDetectWindow)");

            window.ViewModel.SetValues(adDetect.Values ?? new List<DetectValue>());
            window.ViewModel.SelectedDetectTypeIndex = (int)adDetect.Type;

            int index = Detects.IndexOf(adDetect);
            

            window.ShowDialog();

            adDetect.Values = window.ViewModel.GetValues();
            adDetect.Type = (AdDetectType)window.ViewModel.SelectedDetectTypeIndex;

            Detects.RemoveAt(index);
            Detects.Insert(index, adDetect);
        }

        [RelayCommand]
        public void AddDetect()
        {
            NewDetectWindow? window = _fooManager.ServiceProvider.GetService(typeof(NewDetectWindow)) as NewDetectWindow;
            if (window == null)
                throw new Exception("Can not get service (NewDetectWindow)");

            window.ShowDialog();
            List<DetectValue> values = window.ViewModel.GetValues();
            AdDetectType type = (AdDetectType)window.ViewModel.SelectedDetectTypeIndex;

            AdDetect adDetect = new AdDetect()
            {
                Type = type,
                Values = values
            };
            Detects.Add(adDetect);
        }
    }
}
