﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClicker_WPF.Models;
using SmartClicker_WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmartClicker_WPF.ViewModels
{
    [ObservableObject]
    public partial class NewDetectVM
    {
        private AdDetectService _adDetectService;
        public NewDetectVM(AdDetectService AdDetectService)
        {
            _adDetectService = AdDetectService;
            DetectTypes = new ObservableCollection<string>(_adDetectService.GetStringAddDetectTypes());
            Values = new ObservableCollection<DetectValue>();
            SelectedDetectTypeIndex = 0;
            NewValue = String.Empty;
        }

        public void SetValues(List<DetectValue> InValues)
        {
            Values = new ObservableCollection<DetectValue>(InValues);
        }

        public List<DetectValue> GetValues() => Values.ToList();

        [ObservableProperty]
        private ObservableCollection<string> _detectTypes;
        [ObservableProperty]
        private int _selectedDetectTypeIndex;

        [ObservableProperty]
        private ObservableCollection<DetectValue> _values;
        [ObservableProperty]
        private int _selectedValueIndex;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddNewValueCommand))]
        private string _newValue;

        [RelayCommand(CanExecute = nameof(CanRemoveSelected))]
        public void RemoveSelectedValue()
        {
            Values.RemoveAt(SelectedValueIndex);
            SelectedValueIndex = -1;
        }
        bool CanRemoveSelected() => SelectedValueIndex != -1 && Values.Count > 0;

        [RelayCommand(CanExecute = nameof(CanAddNewValue))]
        public void AddNewValue()
        {
            Values.Add(new DetectValue() { Header = NewValue });
            NewValue = string.Empty;
        }
        public bool CanAddNewValue() => !string.IsNullOrEmpty(NewValue);

        [RelayCommand]
        public void RemoveValue(object sender)
        {
            DetectValue? detectValue = sender as DetectValue;
            if(detectValue != null)
                Values.Remove(detectValue);
        }
    }
}
