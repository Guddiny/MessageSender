using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using MessageSender.Services.Interaction;
using MessageSender.State;
using MessageSender.Utils;
using MessageSender.Utils.ActionWrapper;
using MessageSender.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MessageSender.ViewModels.Pages;

public partial class SenderViewModel : ViewModelBase
{
    private readonly MqttService _mqttService;
    private readonly ActionDispatcher _dispatcher;

    private IMessagingService _activeMessagingService;

    private static JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    [ObservableProperty]
    private string _connectButtonText = "Connect";

    [ObservableProperty]
    private TextDocument _incomingMessage = new("{}");

    [ObservableProperty]
    private DeviceMessage? _selectedMessage;

    public SenderViewModel(MqttService mqttService, AppState appState, ActionDispatcher dispatcher)
    {
        _mqttService = mqttService;
        _dispatcher = dispatcher;

        _activeMessagingService = _mqttService; // To support multiple service like MQTT , AMQP and etc.
        _activeMessagingService.DisconnectedFromMessageSource = Disconnected;
        _activeMessagingService.ConnectedToMessageSource = Connected;
        _activeMessagingService.MessageReceived = MessageReceived;

        AppState = appState;
    }

    public AppState AppState { get; private set; }

    [RelayCommand]
    public async Task Connect()
    {
        await _dispatcher
            .Action(async () =>
            {
                if (AppState.AppData.IsDeviceConnected)
                {
                    await _mqttService.Disconnect();
                    IsBackgroundTaskRunning = false;
                }
                else
                {
                    await _mqttService
                        .Connect(
                            AppState.AppData.SelectedDevice!.DeviceId,
                            AppState.AppData.SelectedDevice.Key,
                            AppState.AppData.SelectedDevice.ServerHost);
                    IsBackgroundTaskRunning = true;
                }
            })
            .Run();
    }

    [RelayCommand]
    public async Task SendMessage()
    {
        await _dispatcher
            .Action(async () =>
            {
                var deviceMessage = PrepareDeviceMessage();
                await _mqttService.SendMessage(deviceMessage);
                AppState.AppData.DeviceMessages.Add(deviceMessage);
            })
            .WithNotification(new("D2C Message", "Message sent", NotificationType.Information))
            .Run();
    }

    [RelayCommand]
    public async Task Beautify()
    {
        await _dispatcher
            .Action(() =>
            {
                AppState.AppData.MessageBody =
                    new TextDocument(JsonNode.Parse(AppState.AppData.MessageBody.Text)!.ToJsonString(_serializerOptions));
                AppState.AppData.UserProperties =
                    new TextDocument(JsonNode.Parse(AppState.AppData.UserProperties.Text)!.ToJsonString(_serializerOptions));

                return Task.CompletedTask;
            })
            .Run();
    }

    [RelayCommand]
    public async Task ClearIncomingMessages()
    {
        await _dispatcher
            .Action(() =>
            {
                AppState.AppData.DeviceMessages.Clear();
                IncomingMessage = new TextDocument("{}");

                return Task.CompletedTask;
            })
            .Run();
    }

    [RelayCommand]
    public async Task SaveAsMessages()
    {
        await _dispatcher
            .Action(async () =>
            {
                var result = await DialogHost.Show(new AddEditMessageViewModel(AppState)
                {
                    Text = "Add new Message",
                    IsFromSender = true,
                    DeviceType = AppState.AppData.SelectedDevice?.DeviceType ?? string.Empty
                });
            })
            .Run();
    }


    private DeviceMessage PrepareDeviceMessage()
    {
        var jsonNode = JsonNode.Parse(AppState.AppData.MessageBody.Text); // To validate that text is valid JSON
        var props = JsonSerializer.Deserialize<Dictionary<string, string>>(AppState.AppData.UserProperties.Text) ?? [];

        return new DeviceMessage
        {
            DeviceId = AppState.AppData.SelectedDevice!.DeviceId,
            Direction = MessageDirection.Out,
            Source = MessageSource.MessageSender,
            Timestamp = DateTimeOffset.Now,
            Payload = jsonNode!.ToJsonString(),
            Properties = props,
        };
    }

    // Event handler
    private void Disconnected(object? sender, string? args)
    {
        AppState.AppData.IsDeviceConnected = false;
        ConnectButtonText = "Connect";
        Dispatcher.UIThread.Post(() =>
            Infrastructure
                .GlobalNotificationManager
                .Show(new Notification("Disconnected", "Device disconnected", NotificationType.Information)));
    }

    // Event handler
    private void Connected(object? sender, string? args)
    {
        AppState.AppData.IsDeviceConnected = true;
        ConnectButtonText = "Disconnect";
        Dispatcher.UIThread.Post(() =>
            Infrastructure
                .GlobalNotificationManager
                .Show(new Notification("Connected", "Device connected", NotificationType.Success)));
    }

    // Event handler
    private void MessageReceived(object? sender, DeviceMessage? args)
    {
        if (args is null)
        {
            return;
        }

        args.Source = MessageSource.Cloud;
        args.Timestamp = DateTimeOffset.Now;
        args.Direction = MessageDirection.In;
        AppState.AppData.DeviceMessages.Add(args);
    }

    partial void OnSelectedMessageChanged(DeviceMessage? value)
    {
        if (value is null)
        {
            return;
        }

        try
        {
            var formattedPayload = value.Payload is null
                 ? JsonNode.Parse("{}")
                 : JsonNode.Parse(value.Payload);

            IncomingMessage =
                new TextDocument(JsonSerializer.Serialize(
                    new
                    {
                        payload = formattedPayload,
                        userPoperties = value.Properties
                    },
                    _serializerOptions));
        }
        catch (Exception e)
        {
            Dispatcher.UIThread.Post(() =>
               Infrastructure
                   .GlobalNotificationManager
                   .Show(new Notification("Error", e.Message, NotificationType.Error)));
        }
    }
}