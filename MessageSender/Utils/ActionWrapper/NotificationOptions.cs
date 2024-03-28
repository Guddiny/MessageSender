using Avalonia.Controls.Notifications;

namespace MessageSender.Utils.ActionWrapper;

public class NotificationOptions
{
    public NotificationOptions(string title, string message, NotificationType notificationType)
    {
        Title = title;
        Message = message;
        NotificationType = notificationType;
    }

    public string Title { get; } = string.Empty;

    public string Message { get; } = string.Empty;

    public NotificationType NotificationType { get; }
}
