using System;
using System.Threading.Tasks;

namespace MessageSender.Services.Interaction;

public interface IMessagingService
{
    event Func<DeviceMessage, Task> SomeMessageReceived;

    Task Connect(string deviceId, string deviceKey, string host);

    Task Disconnect();

    bool IsConnected { get; }

    EventHandler<DeviceMessage>? MessageReceived { get; set; }

    EventHandler<string>? ConnectedToMessageSource { get; set; }

    EventHandler<string>? DisconnectedFromMessageSource { get; set; }
}