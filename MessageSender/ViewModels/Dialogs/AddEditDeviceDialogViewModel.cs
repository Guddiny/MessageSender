using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using MessageSender.Shared;
using MessageSender.State;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MessageSender.ViewModels.Dialogs
{
    public partial class AddEditDeviceDialogViewModel : ObservableValidator
    {
        [Required]
        [MinLength(1, ErrorMessage = $" {nameof(DeviceId)} is required!")]
        [ObservableProperty]
        private string _deviceId = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = $" {nameof(ServerHost)} is required!")]
        [ObservableProperty]
        private string _serverHost = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = $" {nameof(Key)} is required!")]
        [ObservableProperty]
        private string _key = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = $" {nameof(Environment)} is required!")]
        [ObservableProperty]
        private string _environment = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = $" {nameof(DeviceType)} is required!")]
        [ObservableProperty]
        private string _deviceType = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _text;

        public AppState AppState { get; set; }

        public AddEditDeviceDialogViewModel(AppState appState)
        {
            AppState = appState;

            SetAutocompletes();
        }

        private void SetAutocompletes()
        {
            DeviceTypeAutocomplete = AppState!.AppData.Devices.Select(d => d.DeviceType).ToArray();
            EnvironmentAutocomplete = AppState!.AppData.Devices.Select(d => d.Environment).ToArray();
            ServerHostAutocomplete = AppState!.AppData.Devices.Select(d => d.ServerHost).ToArray();
        }

        public string[] DeviceTypeAutocomplete { get; set; } = [];

        public string[] EnvironmentAutocomplete { get; set; } = [];

        public string[] ServerHostAutocomplete { get; set; } = [];

        public Guid Id { get; set; }

        [RelayCommand]
        private void Ok()
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                return;
            }

            AddOrUpdateDevice();

            DialogHost.GetDialogSession("TopDialog")?.Close(DialogHostResult.OK);
        }

        private void AddOrUpdateDevice()
        {
            var device = AppState.AppData.Devices
                .FirstOrDefault(
                    d => d.Id == Id);

            if (device != null)
            {
                FillDeviceProperties(device);
                return;
            }

            var deviceToSave = FillDeviceProperties(new Models.Device());
            AppState.AppData.Devices.Add(deviceToSave);
        }

        private Models.Device FillDeviceProperties(Models.Device device)
        {
            device.DeviceId = DeviceId;
            device.DeviceType = DeviceType;
            device.Environment = Environment;
            device.Key = Key;
            device.ServerHost = ServerHost;
            device.Description = Description;

            return device;
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogHost.GetDialogSession("TopDialog")?.Close(DialogHostResult.Cancel);
        }
    }
}
