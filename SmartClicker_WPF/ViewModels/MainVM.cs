using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClicker_WPF.Interfaces;
using SmartClicker_WPF.Models;
using SmartClicker_WPF.Services;
using SmartClicker_WPF.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SmartClicker_WPF.ViewModels
{
    [ObservableObject]
    public partial class MainVM
    {
        private SettingsService _settingsService;
        public SettingsJson SettingsJson { get; private set; }
        private FooManager _fooManager;
        private ProxyService _proxyService;
        private WebService _webService;
        private InputService _inputService;
        private CancellationTokenSource? _cancellationTokenSource;
        public WebTasker? Tasker { get; private set; }

        public MainVM(WebService webService, InputService inputService, SettingsService settingsService, FooManager fooManager, ProxyService proxyService)
        {
            _webService = webService;
            _settingsService = settingsService;
            _fooManager = fooManager;
            _proxyService = proxyService;
            _inputService = inputService;
            SettingsJson = _settingsService.GetSettingsObject();

            Drivers = new ObservableCollection<Driver>(_settingsService.GetDrivers(SettingsJson));
            Detects = new ObservableCollection<AdDetect>();
            ProxyTypes = new ObservableCollection<string>(_proxyService.GetProxyTypesString());
            Logs = new ObservableCollection<LogModel>();

            SelectedProxyTypeIndex = 0;
            SelectedDriver = Drivers[0];
            TimeOut = 60;
            Loops = 5;
            SiteUrl = @"101gardentools.com";
            KeyWords = "";
            CurrentIteration = 0;
            TotalClicks = 0;
            Status = "None";
            CurrentProxy = "None";
        }

        [ObservableProperty]
        private bool _useProxy;

        [ObservableProperty]
        private bool _checkProxy;

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

        [ObservableProperty]
        private int _selectedLogIndex;

        [ObservableProperty]
        private int _currentIteration;

        [ObservableProperty]
        private int _totalClicks;

        [ObservableProperty]
        private string _status;

        [ObservableProperty]
        private string _currentProxy;

        [ObservableProperty]
        private bool _clickAd;

        [RelayCommand]
        private void RemoveDetect(object sender)
        {
            if (Detects.Count <= 0)
                throw new Exception("No list box items (Ad detects)");

            AdDetect? selected = sender as AdDetect;
            if (selected == null)
                throw new Exception("Command sender is not AdDetect");

            Detects.Remove(selected);
        }

        [RelayCommand]
        private void AddDetect()
        {
            AdDetect adDetect = new AdDetect();
            Detects.Add(adDetect);
        }

        [RelayCommand]
        private async Task OpenDriverSettings()
        {
            DriverSettingsWindow? window = _fooManager.ServiceProvider.GetService(typeof(DriverSettingsWindow)) as DriverSettingsWindow;
            if (window == null)
                throw new Exception("Can not get service (DriverSettingsWindow)");

            int selected = Drivers.IndexOf(SelectedDriver);

            await OpenSettingsWindow(window);

            Drivers = new ObservableCollection<Driver>(_settingsService.GetDrivers(SettingsJson));
            SelectedDriver = Drivers[selected];
        }

        [RelayCommand]
        private async Task OpenClickerSettings()
        {
            ClickerSettingsWindow? window = _fooManager.ServiceProvider.GetService(typeof(ClickerSettingsWindow)) as ClickerSettingsWindow;
            if (window == null)
                throw new Exception("Can not get service (ClickerSettingsWindow)");

            await OpenSettingsWindow(window);
        }

        [RelayCommand]
        private void Cancel()
        {
            if(Tasker == null || (Tasker != null && !Tasker.IsInProgress))
            {
                throw new Exception("No process");
            }
            if(_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        [RelayCommand(CanExecute = nameof(CanStart))]
        private async Task Start()
        {
            CheckBeforeStart();
        
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken ct = _cancellationTokenSource.Token;

            //Clear logs
            Logs = new ObservableCollection<LogModel>();
            
            if (_useProxy)
            {
                var proxies = _proxyList.Split("\r\n");
                Tasker = new WebTasker(ct,
                   _webService,
                   _inputService,
                   _siteUrl,
                   _keyWords,
                   GetDriverPath(),
                   _timeOut,
                   (WebDriverType)(Drivers.IndexOf(SelectedDriver)),
                   _loops,
                   _detects,
                   _proxyService,
                   _checkProxy,
                   proxies,
                   (WebProxyType)(_selectedProxyTypeIndex),
                   string.IsNullOrEmpty(_proxyUserName) ? null : _proxyUserName,
                   string.IsNullOrEmpty(_proxyPassword) ? null : _proxyPassword);
            }
            else
            {
                Tasker = new WebTasker(ct,
                   _webService,
                   _inputService,
                   _siteUrl,
                   _keyWords,
                   GetDriverPath(),
                   _timeOut,
                   (WebDriverType)(Drivers.IndexOf(SelectedDriver)),
                   _loops,
                   _detects);
            }
            // Init values
            Tasker.MaxPageCount = SettingsJson.MaxPageCount;
            Tasker.FindCookieButtonTimeOutS = SettingsJson.TimeOut_CookieButton;
            Tasker.FindSearchBarTimeOutS = SettingsJson.TimeOut_SearchBar;
            Tasker.FindSearchButtonTimeOutS = SettingsJson.TimeOut_SearchButton;
            Tasker.NavTableSearchTiemOutS = SettingsJson.TimeOut_NavTable;
            Tasker.PageSearchTimeOutS = SettingsJson.TimeOut_Page;
            Tasker.PageLoadTimeOutS = SettingsJson.TimeOut_PageLoad;
            Tasker.TimeOutFindLink = SettingsJson.TimeOut_FindLink;
            Tasker.TimeOutLinkClick = SettingsJson.TimeOut_LinkClick;
            Tasker.MaxPageCount = SettingsJson.MaxPageCount;
            Tasker.DelayBetweenActivity = SettingsJson.DelayBetweenActivityMs;
            Tasker.RandomDelayBetweenActivity = SettingsJson.RandomDelayBetweenActivity;
            Tasker.HideBrowser = SettingsJson.HideBrowser;
            Tasker.HideConsole = SettingsJson.HideConsole;

            // Subscribe
            Tasker.OnAdClicksChanged += Tasker_OnAdClicksChanged;
            Tasker.OnStatusChanged += Tasker_OnStatusChanged;
            Tasker.OnIterationChanged += Tasker_OnIterationChanged;
            Tasker.OnUsingProxy += Tasker_OnUsingProxy;
            Tasker.OnFinished += Tasker_OnFinished;
            Tasker.OnCompleted += Tasker_OnCompleted;
            Tasker.OnLog += Tasker_OnLog;

            CurrentIteration = 0;
            Status = ((WebTaskerState)(0)).ToString();
            TotalClicks = 0;

            await Tasker.StartWork();
        }

        private async Task OpenSettingsWindow(ISettingsWindow window)
        {
            window.InsertSettings(SettingsJson);
            window.ShowAsDialog();
            window.ModifySettings(SettingsJson);
            await _settingsService.SaveSettingsObjectAsync(SettingsJson);
        }


        private void Tasker_OnUsingProxy(string proxy)
        {
            CurrentProxy = proxy;
        }

        public bool CanStart()
        {
            if (Tasker == null)
                return false;
            return !Tasker.IsInProgress;
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
                throw new Exception("Ad filter not configured");

            Tasker = null;
        }

        private void AddLog(string text)
        {
            Logs.Add(new LogModel() { Log = text });
            SelectedLogIndex = (Logs.Count - 1);
        }
        

        private void Tasker_OnLog(string log)
        {
            AddLog($"{_selectedDriver.Title} driver: {log}");
        }

        private void Tasker_OnFinished(string reason)
        {
            AddLog($"Finished: {reason}" );
            
        }

        private void Tasker_OnCompleted()
        {
            CurrentProxy = "None";
            Status = ((WebTaskerState)(0)).ToString();
            CurrentIteration = 0;
            _cancellationTokenSource = null;
        }

        private void Tasker_OnIterationChanged(int iteration)
        {
            CurrentIteration = iteration + 1;
        }

        private void Tasker_OnStatusChanged(string status)
        {
            Status = status;
        }

        private void Tasker_OnAdClicksChanged(int adClicks)
        {
            TotalClicks = adClicks;
        }


        //TODO: To service
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
