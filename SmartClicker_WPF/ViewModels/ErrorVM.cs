﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace SmartClicker_WPF.ViewModels
{
    [ObservableObject]
    public partial class ErrorVM
    {
        public ErrorVM()
        {
            ErrorTitle = "Error";
            Message = "Unknwon";
        }

        public ErrorVM(string Title, string Msg)
        {
            ErrorTitle = Title;
            Message = Msg;
        }

        [ObservableProperty]
        private string? _errorTitle;
        [ObservableProperty]
        private string? _message;
    }
}
