using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Text.Json.Serialization;

namespace MessageSender.Models;

public partial class Device : ObservableObject
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("deviceId")]
    [ObservableProperty]
    private string _deviceId = string.Empty;

    [ObservableProperty]
    private string _environment = string.Empty;

    [ObservableProperty]
    private string _deviceType = string.Empty;

    [ObservableProperty]
    private string _serverHost = string.Empty;

    [ObservableProperty]
    private string _key = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;
}