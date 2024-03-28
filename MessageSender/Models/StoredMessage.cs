using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace MessageSender.Models;

public partial class StoredMessage : ObservableObject
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _deviceType = string.Empty;

    [ObservableProperty]
    private TextDocument _messageBody = new TextDocument("{}");

    [ObservableProperty]
    private TextDocument _userProperties = new TextDocument("{}");
}
