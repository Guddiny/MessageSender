using CommunityToolkit.Mvvm.ComponentModel;
using MessageSender.State;
using MessageSender.ViewModels.Controls;
using System.Reflection;

namespace MessageSender.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public AppState AppState { get; }

    public SideBarViewModel SideBarViewModel { get; }

    public string Title { get; }

    public MainViewModel(AppState appState, SideBarViewModel sideBarViewModel)
    {
        AppState = appState;
        SideBarViewModel = sideBarViewModel;

        var version = Assembly.GetExecutingAssembly().GetName().Version;

        Title = $"MessageSender - {version}";
    }
}