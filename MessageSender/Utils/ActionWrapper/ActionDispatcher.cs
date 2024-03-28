using MessageSender.State;
using System;
using System.Threading.Tasks;

namespace MessageSender.Utils.ActionWrapper
{
    public class ActionDispatcher
    {
        private readonly AppState _appState;

        public ActionDispatcher(AppState appState)
        {
            _appState = appState;
        }

        public TrackedActionBuilder Action(Func<Task> func)
        {
            var actionBuilder = new TrackedActionBuilder(_appState);
            actionBuilder.Action(func);

            return actionBuilder;
        }
    }
}
