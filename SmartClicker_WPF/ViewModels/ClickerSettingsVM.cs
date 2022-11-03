using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            
        }

        public void ModifySettings(SettingsJson settingsJson)
        {
            
        }

        [RelayCommand]
        public void Reset()
        {

        }


        
    }
}
