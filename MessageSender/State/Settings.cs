using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MessageSender.State;

public partial class Settings : ObservableObject
{
    [ObservableProperty]
    private string _themeVariant = "Light";

    [ObservableProperty]
    private ThemeVariant _variant;
}
