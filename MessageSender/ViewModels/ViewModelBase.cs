using CommunityToolkit.Mvvm.ComponentModel;

namespace MessageSender.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isBackgroundTaskRunning = false;
}
