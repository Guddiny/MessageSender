using Avalonia.Controls.Notifications;

namespace MessageSender.Utils;

public static class Infrastructure
{
    public static WindowNotificationManager GlobalNotificationManager { get; set; } = new(null);
}
