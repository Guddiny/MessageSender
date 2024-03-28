using System;
using System.Collections.Generic;

namespace MessageSender.Services.Interaction;

public class DeviceMessage
{
    public string DeviceId { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public Dictionary<string, string> Properties { get; set; } = new();

    public DateTimeOffset Timestamp { get; set; }

    public MessageSource Source { get; set; }

    public MessageDirection Direction { get; set; }

    public string Method => Properties.TryGetValue("method", out string? method)
        ? method
        : string.Empty;
}