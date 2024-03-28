using Avalonia.Collections;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using MessageSender.Models;
using MessageSender.State;
using MessageSender.Utils.ActionWrapper;
using MessageSender.ViewModels.Dialogs;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MessageSender.ViewModels.Pages;

public partial class MessageManagementViewModel : ViewModelBase
{
    private readonly ActionDispatcher _dispatcher;

    [ObservableProperty]
    private StoredMessage _selectedMessage;

    [ObservableProperty]
    private TextDocument _selectedMessageBody = new("{}");

    [ObservableProperty]
    private TextDocument _selectedMessageUserProperties = new("{}");

    [ObservableProperty]
    private bool _canDelete;

    private static JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public MessageManagementViewModel(AppState appState, ActionDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        AppState = appState;

        FillDataGreed();
    }

    public AppState AppState { get; set; }

    public DataGridCollectionView MessagesDataGrid { get; set; }

    private void FillDataGreed()
    {
        MessagesDataGrid = new DataGridCollectionView(AppState.AppData.Messages);
        MessagesDataGrid.GroupDescriptions.Add(new DataGridPathGroupDescription(nameof(StoredMessage.DeviceType)));
    }

    [RelayCommand]
    private async Task DeleteMessage()
    {
        await _dispatcher
            .Action(() =>
            {
                var msg = AppState.AppData.Messages.FirstOrDefault(m => m.Id == SelectedMessage.Id);
                AppState.AppData.Messages.Remove(msg);

                return Task.CompletedTask;
            })
            .WithConfirmationDialog(new DialogOptions() { Message = $"Do you want to remove the message '{SelectedMessage?.Name}'" })
            .Run();
    }

    [RelayCommand]
    private async Task AddMessage()
    {
        await _dispatcher
            .Action(async () =>
            {
                var result = await DialogHost.Show(new AddEditMessageViewModel(AppState) { Text = "Add new Message" });
            })
            .Run();
    }

    [RelayCommand]
    public async Task Beautify()
    {
        await _dispatcher
            .Action(() =>
            {
                SelectedMessage.MessageBody =
                    new TextDocument(JsonNode.Parse(SelectedMessage.MessageBody.Text)!.ToJsonString(_serializerOptions));
                SelectedMessage.UserProperties =
                    new TextDocument(JsonNode.Parse(SelectedMessage.UserProperties.Text)!.ToJsonString(_serializerOptions));

                return Task.CompletedTask;
            })
            .Run();
    }

    [RelayCommand]
    public async Task SetInSender()
    {
        await _dispatcher
            .Action(() =>
            {
                var body =
                    new TextDocument(JsonNode.Parse(SelectedMessage.MessageBody.Text)!.ToJsonString(_serializerOptions));
                var userProperties =
                     new TextDocument(JsonNode.Parse(SelectedMessage.UserProperties.Text)!.ToJsonString(_serializerOptions));

                AppState.AppData.MessageBody = body;
                AppState.AppData.UserProperties = userProperties;

                return Task.CompletedTask;
            })
            .WithConfirmationDialog(new DialogOptions() { Message = $"Copy this '{SelectedMessage?.Name}' to Sender Page?" })
            .Run();
    }

    partial void OnSelectedMessageChanged(StoredMessage value)
    {
        if (value == null)
        {
            CanDelete = false;
            return;
        }

        CanDelete = true;
        SelectedMessageBody = new TextDocument(value.MessageBody);
        SelectedMessageUserProperties = new TextDocument(value.UserProperties);
    }
}
