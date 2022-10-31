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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        private WebService _webService;
        private InputService _inputService;
        public WebTasker Tasker { get; private set; }

        public MainVM(WebService WebService, InputService inputService, SettingsService SettingsService, FooManager FooManager, ProxyService ProxyService)
        {
            _webService = WebService;
            _settingsService = SettingsService;
            _fooManager = FooManager;
            _proxyService = ProxyService;
            _inputService = inputService;
            _settingsJson = _settingsService.GetSettingsObject();

            Drivers = new ObservableCollection<Driver>(_settingsService.GetDrivers(_settingsJson));
            Detects = new ObservableCollection<AdDetect>();
            ProxyTypes = new ObservableCollection<string>(_proxyService.GetProxyTypesString());
            Logs = new ObservableCollection<LogModel>();

            SelectedProxyTypeIndex = 0;
            SelectedDriver = Drivers[0];
            TimeOut = 60;
            Loops = 5;
            SiteUrl = @"101gardentools.com";
            KeyWords = "";
            InProgress = false;
        }

        public bool IsStartButtonEnabled => !InProgress;

        [ObservableProperty]
        private bool _inProgress;

        [ObservableProperty]
        private bool _useProxy;

        [ObservableProperty]
        private string _proxyList;

        [ObservableProperty]
        private ObservableCollection<string> _proxyTypes;

        [ObservableProperty]
        private int _selectedProxyTypeIndex;

        [ObservableProperty]
        private string _keyWords;

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

        [ObservableProperty]
        private ObservableCollection<LogModel> _logs;

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
        public void AddDetect()
        {
            AdDetect adDetect = new AdDetect();
            Detects.Add(adDetect);
        }

        [RelayCommand]
        public async Task OpenDriverSettings()
        {
            DriverSettingsWindow? window = _fooManager.ServiceProvider.GetService(typeof(DriverSettingsWindow)) as DriverSettingsWindow;
            if (window == null)
                throw new Exception("Can not get service (DriverSettingsWindow)");
            int selected = Drivers.IndexOf(SelectedDriver);

            window.ViewModel.InsertSettings(_settingsJson);
            window.ShowDialog();
            window.ViewModel.ModifySettings(_settingsJson);
            await _settingsService.SaveSettingsObjectAsync(_settingsJson);

            Drivers = new ObservableCollection<Driver>(_settingsService.GetDrivers(_settingsJson));
            SelectedDriver = Drivers[selected];
        }

        [RelayCommand]
        public async Task Start()
        {
            if(true)
            {
                var proxy = _proxyList.Split("\r\n")[0];
                MessageBox.Show((await _proxyService.CheckProxy(proxy, 5000)).ToString());
                return;
            }
            CheckBeforeStart();
            //_cancelTokenSource = new CancellationTokenSource();
            Tasker = new WebTasker(
                _webService,
                _inputService,
                _siteUrl, 
                _keyWords, 
                GetDriverPath(), 
                _timeOut, 
                (WebDriverType)(Drivers.IndexOf(SelectedDriver)), 
                _loops);
            Tasker.MaxPageCount = 20;
            InProgress = true;
            Tasker.OnFinished += Tasker_OnFinished;
            Tasker.OnLog += Tasker_OnLog;

            //_cancelTokenSource.CancelAfter(TimeOut * 6 * 1000);
            await Tasker.Run();
        }

        private void CheckBeforeStart()
        {
            if (UseProxy && string.IsNullOrEmpty(ProxyList)) 
                throw new Exception("Proxy list is empty");
            if (string.IsNullOrEmpty(KeyWords)) 
                throw new Exception("Keywords are empty");
            if (string.IsNullOrEmpty(SiteUrl)) 
                throw new Exception("Site url is empty");
            if (_loops <= 0) 
                throw new Exception("Incorrect format: Loops");
            if (SelectedDriver == null) 
                throw new Exception("Selected driver is null");
            if (Detects.Count <= 0) 
                throw new Exception("Ad detection not configured");
        }

        private void Tasker_OnLog(string log)
        {
            Logs.Add(new LogModel() { Log = $"{_selectedDriver.Title} driver: {log}" });
        }

        private void Tasker_OnFinished(string reason)
        {
            InProgress = false;
            throw new Exception($"Task finished, reason: {reason}");
        }

        private string GetDriverPath()
        {
            string fullPath = _selectedDriver.Path ?? "";
            if (Directory.Exists(fullPath))
            {
                return fullPath;
            }
            if (File.Exists(fullPath))
            {
                string newPath = Path.GetFullPath(Path.Combine(fullPath, @"..\"));
                if (Directory.Exists(newPath))
                {
                    return newPath;
                }
            }
            throw new Exception("Invalid path for web driver");
        }
    }
}
