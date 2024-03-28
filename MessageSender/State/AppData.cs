using Avalonia.Collections;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using MessageSender.Models;
using MessageSender.Services.Interaction;
using System.Collections.ObjectModel;

namespace MessageSender.State;

public partial class AppData : ObservableObject
{
    [ObservableProperty]
    private bool _isDeviceConnected;

    [ObservableProperty]
    private TextDocument _messageBody = new("{}");

    [ObservableProperty]
    private TextDocument _userProperties = new("{}");

    [ObservableProperty]
    private Device? _selectedDevice;

    [ObservableProperty]
    private int? _selectedDeviceIndex;

    [ObservableProperty]
    private bool _isProgressVisible;

    public ObservableCollection<Device> Devices { get; set; } = [];

    public ObservableCollection<StoredMessage> Messages { get; set; } = [];

    public AvaloniaList<DeviceMessage> DeviceMessages { get; set; } = [];

    public void ShowProgress()
    {
        IsProgressVisible = true;
    }

    public void HideProgress()
    {
        IsProgressVisible = false;
    }
}