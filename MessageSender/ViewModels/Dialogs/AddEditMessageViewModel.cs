using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using MessageSender.Shared;
using MessageSender.State;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MessageSender.ViewModels.Dialogs
{
    public partial class AddEditMessageViewModel : ObservableValidator
    {
        

        [ObservableProperty]
        private string _text = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = $" {nameof(DeviceType)} is required!")]
        [ObservableProperty]
        private string _deviceType = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = $" {nameof(MessageName)} is required!")]
        [ObservableProperty]
        private string _messageName = string.Empty;

        public AddEditMessageViewModel(AppState appState)
        {
            AppState = appState;
            DeviceTypeAutocomplete = AppState.AppData.Messages.Select(m => m.DeviceType).ToArray();
            MessageNameAutocomplete = AppState.AppData.Messages.Select(m => m.Name).ToArray();
        }

        public AppState AppState { get; set; }

        public string[] DeviceTypeAutocomplete { get; set; } = [];

        public string[] MessageNameAutocomplete { get; set; } = [];

        public bool IsFromSender { get; set; }

        [RelayCommand]
        private void Ok()
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                return;
            }

            var newMessage = new Models.StoredMessage { DeviceType = DeviceType, Name = MessageName };

            if (IsFromSender)
            {
                newMessage.MessageBody = new TextDocument(AppState.AppData.MessageBody.Text);
                newMessage.UserProperties = new TextDocument(AppState.AppData.UserProperties.Text);
            }

            AppState.AppData.Messages.Add(newMessage);

            DialogHost.GetDialogSession("TopDialog")?.Close(DialogHostResult.OK);
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogHost.GetDialogSession("TopDialog")?.Close(DialogHostResult.Cancel);
        }
    }
}
