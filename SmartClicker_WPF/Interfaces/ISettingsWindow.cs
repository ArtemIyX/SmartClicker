using SmartClicker_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Interfaces
{
    internal interface ISettingsWindow
    {
        public void InsertSettings(SettingsJson settingsJson);
        public void ModifySettings(SettingsJson settingsJson);
        public void ShowAsDialog();
    }
}
