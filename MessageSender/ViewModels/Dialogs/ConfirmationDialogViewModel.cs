using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using MessageSender.Shared;

namespace MessageSender.ViewModels.Dialogs;

public partial class ConfirmationDialogViewModel : ViewModelBase
{
    public string Text { get; set; } = "";

    [RelayCommand]
    public void Ok()
    {
        DialogHost.GetDialogSession("TopDialog")?.Close(DialogHostResult.OK);
    }

    [RelayCommand]
    public void Cancel()
    {
        DialogHost.GetDialogSession("TopDialog")?.Close(DialogHostResult.Cancel);
    }
}
