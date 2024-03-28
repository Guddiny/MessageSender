using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using System.Threading.Tasks;

namespace MessageSender.Utils.ActionWrapper
{
    public class NotificationDecorator : ITrackedAction
    {
        private readonly ITrackedAction _trackedAction;
        private readonly NotificationOptions _notificationOptions;

        public NotificationDecorator(ITrackedAction trackedAction, NotificationOptions notificationOptions)
        {
            _trackedAction = trackedAction;
            _notificationOptions = notificationOptions;
        }

        public async Task Run()
        {
            await _trackedAction.Run();
            Dispatcher.UIThread.Post(() =>
                Infrastructure
                    .GlobalNotificationManager
                    .Show(new Notification(_notificationOptions.Title, _notificationOptions.Message, _notificationOptions.NotificationType)));
        }
    }
}
