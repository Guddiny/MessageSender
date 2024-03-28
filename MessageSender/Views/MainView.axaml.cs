using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using MessageSender.Utils;

namespace MessageSender.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Infrastructure.GlobalNotificationManager = new WindowNotificationManager(TopLevel.GetTopLevel(this));
    }
}
