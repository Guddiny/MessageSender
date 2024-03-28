using CommunityToolkit.Mvvm.ComponentModel;
using MessageSender.Services.Routing;
using MessageSender.ViewModels.Dialogs;
using MessageSender.ViewModels.Pages;
using MessageSender.Views.Pages;
using System.Collections.Generic;

namespace MessageSender.ViewModels.Controls;

public partial class SideBarViewModel : ViewModelBase
{
    private readonly HistoryRouter<ViewModelBase> _router;

    [ObservableProperty]
    private NavigationItemViewModel _currentPage = default!;

    [ObservableProperty]
    private ViewModelBase _page = default!;

    public SideBarViewModel(HistoryRouter<ViewModelBase> router)
    {
        _router = router;
        _router.CurrentViewModelChanged += Router_CurrentViewModelChanged;
    }

    private void Router_CurrentViewModelChanged(ViewModelBase obj)
    {
        Page = obj;
    }

    public List<NavigationItemViewModel> Navigations =>
        [
            new(nameof(SenderView), "Send", "SendRegular"),
            new(nameof(DeviceManagementView), "Device", "DeviceRegular"),
            new(nameof(MessageManagementView), "Message", "MessageRegular"),
            //new(nameof(LogManagementView), "Log", "LogRegular"),
        ];

    partial void OnCurrentPageChanged(NavigationItemViewModel? oldValue, NavigationItemViewModel newValue)
    {
        ViewModelBase vm = newValue.View switch
        {
            nameof(SenderView) => _router.GoTo<SenderViewModel>(),
            nameof(DeviceManagementView) => _router.GoTo<DeviceManagementViewModel>(),
            nameof(MessageManagementView) => _router.GoTo<MessageManagementViewModel>(),
            nameof(LogManagementView) => _router.GoTo<LogManagementViewModel>(),
            _ => new ConfirmationDialogViewModel()
        };

        newValue.ViewModel = vm;
    }
}