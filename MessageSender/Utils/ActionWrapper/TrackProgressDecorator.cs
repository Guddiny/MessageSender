using MessageSender.State;
using System.Threading.Tasks;

namespace MessageSender.Utils.ActionWrapper
{
    internal class TrackProgressDecorator : ITrackedAction
    {
        private readonly ITrackedAction _trackedAction;
        private readonly AppState _appState;

        public TrackProgressDecorator(
            ITrackedAction trackedAction,
            AppState appState)
        {
            _trackedAction = trackedAction;
            _appState = appState;
        }

        public async Task Run()
        {
            _appState.AppData.ShowProgress();
            await _trackedAction.Run();
            _appState.AppData.HideProgress();
        }
    }
}
