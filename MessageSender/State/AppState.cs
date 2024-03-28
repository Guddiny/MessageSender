using MessageSender.ViewModels;

namespace MessageSender.State;

public class AppState : ViewModelBase
{
    public Settings Settings { get; set; } = new();

    public AppData AppData { get; set; } = new();
}
