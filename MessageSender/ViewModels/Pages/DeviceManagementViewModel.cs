using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using MessageSender.Models;
using MessageSender.State;
using MessageSender.Utils.ActionWrapper;
using MessageSender.ViewModels.Dialogs;
using System.Threading.Tasks;

namespace MessageSender.ViewModels.Pages;

public partial class DeviceManagementViewModel : ViewModelBase
{
    private readonly ActionDispatcher _dispatcher;

    public DeviceManagementViewModel(ActionDispatcher dispatcher, AppState appState)
    {
        _dispatcher = dispatcher;
        AppState = appState;

        FillDataGreed();
    }

    [ObservableProperty]
    private Device? _selectedDevice;

    [ObservableProperty]
    private bool _canEditOrDelete;

    public DataGridCollectionView DevicesDataGrid { get; set; }

    public AppState AppState { get; set; }

    private void FillDataGreed()
    {
        DevicesDataGrid = new DataGridCollectionView(AppState.AppData.Devices);
        DevicesDataGrid.GroupDescriptions.Add(new DataGridPathGroupDescription(nameof(Device.DeviceType)));
        DevicesDataGrid.GroupDescriptions.Add(new DataGridPathGroupDescription(nameof(Device.Environment)));
    }

    [RelayCommand]
    private async Task AddNewDeviceDialog()
    {
        await _dispatcher
            .Action(async () =>
            {
                var result = await DialogHost.Show(new AddEditDeviceDialogViewModel(AppState) { Text = "Add new Device" });
            })
            .Run();

    }

    [RelayCommand]
    private async Task EditDevice()
    {
        await _dispatcher
          .Action(async () =>
          {
              var result = await DialogHost.Show(new AddEditDeviceDialogViewModel(AppState)
              {
                  Text = "Edit Device",
                  Id = SelectedDevice.Id,
                  DeviceId = SelectedDevice.DeviceId,
                  DeviceType = SelectedDevice.DeviceType,
                  Environment = SelectedDevice.Environment,
                  ServerHost = SelectedDevice.ServerHost,
                  Key = SelectedDevice.Key,
                  Description = SelectedDevice.Description
              });
          })
          .Run();
    }

    [RelayCommand]
    private async Task DeleteDevice()
    {
        await _dispatcher
            .Action(() =>
            {
                AppState.AppData.Devices.Remove(_selectedDevice);

                return Task.CompletedTask;
            })
            .WithConfirmationDialog(new DialogOptions() { Message = $"Do you want to remove the device {SelectedDevice?.DeviceId}" })
            .Run();
    }

    partial void OnSelectedDeviceChanged(Device? oldValue, Device? newValue)
    {
        if (newValue == null || SelectedDevice == null)
        {
            CanEditOrDelete = false;
        }
        else
        {
            CanEditOrDelete = true;
        }
    }
}
