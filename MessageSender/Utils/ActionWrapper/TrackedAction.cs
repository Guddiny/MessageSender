using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace MessageSender.Utils.ActionWrapper
{
    public class TrackedAction : ITrackedAction
    {
        private readonly Func<Task> _func;

        public TrackedAction(Func<Task> func)
        {
            _func = func;
        }

        public async Task Run()
        {
            try
            {
                await _func();
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
}
