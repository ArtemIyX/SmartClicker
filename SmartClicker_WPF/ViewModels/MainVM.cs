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

namespace SmartClicker_WPF.ViewModels
{
    [ObservableObject]
    public partial class MainVM
    {
        private SettingsService _settingsService;
        private SettingsJson _settingsJson;
        private FooManager _fooManager;
        public MainVM(SettingsService SettingsService, FooManager FooManager)
        {
            _settingsService = SettingsService;
            _fooManager = FooManager;
            _settingsJson = _settingsService.GetSettingsObject();
            _drivers = new ObservableCollection<Driver>(_settingsService.GetDrivers(_settingsJson));
            _selectedDriver = Drivers[0];
            _timeOut = 60;
            _loops = 5;
            _siteUrl = @"101gardentools.com";
        }

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

        [RelayCommand(CanExecute = nameof(CanRemoveDetect))]
        public void RemoveDetect()
        {
            if(SelectedDetect != null)
            {
                Detects.Remove(SelectedDetect);
            }
        }
        public bool CanRemoveDetect => SelectedDetect != null && Detects.Count > 0;

        [RelayCommand]
        public void AddDetect()
        {
            NewDetectWindow? window = _fooManager.ServiceProvider.GetService(typeof(NewDetectWindow)) as NewDetectWindow;
            window?.ShowDialog();
        }
    }
}
