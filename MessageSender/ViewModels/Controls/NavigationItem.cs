using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MessageSender.ViewModels.Controls;

public partial class NavigationItemViewModel : ObservableObject
{
    public NavigationItemViewModel(string view, string title, string iconKey)
    {
        View = view;
        Title = title;

        Application.Current!.TryFindResource(iconKey, out var res);
        Icon = (StreamGeometry)res!;
    }

    [ObservableProperty]
    public ViewModelBase _viewModel = new();

    [ObservableProperty]
    private string _view;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private StreamGeometry _icon;

    [ObservableProperty]
    private bool _hasBackgroundActivity;
}
