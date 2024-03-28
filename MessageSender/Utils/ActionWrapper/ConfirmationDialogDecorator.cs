using DialogHostAvalonia;
using MessageSender.Shared;
using MessageSender.ViewModels.Dialogs;
using System.Threading.Tasks;

namespace MessageSender.Utils.ActionWrapper
{
    public class ConfirmationDialogDecorator : ITrackedAction
    {
        private readonly ITrackedAction trackedAction;
        private readonly DialogOptions dialogOptions;

        public ConfirmationDialogDecorator(ITrackedAction trackedAction, DialogOptions dialogOptions)
        {
            this.trackedAction = trackedAction;
            this.dialogOptions = dialogOptions;
        }

        public async Task Run()
        {
            var box = await DialogHost.Show(new ConfirmationDialogViewModel() { Text = dialogOptions.Message });

            if ((string)box! != DialogHostResult.OK)
            {
                return;
            }

            await trackedAction.Run();
        }
    }
}
