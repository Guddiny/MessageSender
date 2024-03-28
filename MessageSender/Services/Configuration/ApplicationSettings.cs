using MessageSender.Models;
using System.Collections.Generic;

namespace MessageSender.Services.Configuration;

public class ApplicationSettings
{
    public string CurrentMessageBody { get; set; } = "{}";

    public string CurrentMessageUserProperties { get; set; } = "{}";

    public Device? CurrentDevice { get; set; }

    public List<Device> Devices { get; set; } = [];

    public List<StoredMessage> Messages { get; set; } = [];

    public int? CurrentDeviceIndex { get; set; }
}
