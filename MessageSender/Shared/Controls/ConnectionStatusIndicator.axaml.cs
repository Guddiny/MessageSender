using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace MessageSender.Shared.Controls;

public class ConnectionStatusIndicator : TemplatedControl
{
    public new static readonly StyledProperty<IBrush?> BackgroundProperty =
        Border.BackgroundProperty.AddOwner<ConnectionStatusIndicator>();

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<ConnectionStatusIndicator, string>(nameof(Text), "Default Text");

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public new IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }
}