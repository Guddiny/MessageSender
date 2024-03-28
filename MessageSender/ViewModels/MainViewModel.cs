using MessageSender.State;
using MessageSender.ViewModels.Controls;

namespace MessageSender.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public AppState AppState { get; }

    public SideBarViewModel SideBarViewModel { get; }

    public MainViewModel(AppState appState, SideBarViewModel sideBarViewModel)
    {
        AppState = appState;
        SideBarViewModel = sideBarViewModel;
    }
}