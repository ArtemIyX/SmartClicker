using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartClicker_WPF.ViewModels
{
    [ObservableObject]
    public partial class ErrorVM
    {
        public ErrorVM()
        {
            this.ErrorTitle = "Error";
            this.Message = "Unknwon";
        }

        public ErrorVM(string Title, string Msg)
        {
            this.ErrorTitle = Title;
            this.Message = Msg;
        }

        [ObservableProperty]
        private string? _errorTitle;
        [ObservableProperty]
        private string? _message;
    }
}
