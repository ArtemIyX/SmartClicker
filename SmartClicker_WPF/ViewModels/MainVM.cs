using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        [ObservableProperty]
        private string? _test = "Test";

        [RelayCommand(CanExecute = nameof(CanClick))]
        public async Task Click()
        {
            await Task.Delay(1_000);
            Test = "Hello";
        }
        public bool CanClick() => _test == "Test";

    }
}
