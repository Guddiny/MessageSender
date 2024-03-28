using MessageSender.State;
using System;
using System.Threading.Tasks;

namespace MessageSender.Utils.ActionWrapper
{
    public class TrackedActionBuilder
    {
        private readonly AppState _appState;
        private Func<Task> _func;
        private ITrackedAction _trackedAction;

        public TrackedActionBuilder(AppState appState)
        {
            _appState = appState;
        }

        public TrackedActionBuilder Action(Func<Task> func)
        {
            _func = func;
            _trackedAction = new TrackedAction(func);
            return this;
        }

        public TrackedActionBuilder WithConfirmationDialog(DialogOptions dialogOptions)
        {
            _trackedAction = new ConfirmationDialogDecorator(_trackedAction, dialogOptions);
            return this;
        }

        public TrackedActionBuilder WithProgressIndicator()
        {
            _trackedAction = new TrackProgressDecorator(_trackedAction, _appState);
            return this;
        }

        public TrackedActionBuilder WithNotification(NotificationOptions notificationOptions)
        {
            _trackedAction = new NotificationDecorator(_trackedAction, notificationOptions);
            return this;
        }

        public async Task Run()
        {
            await _trackedAction.Run();
        }
    }
}
