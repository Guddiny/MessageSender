using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using System.ComponentModel;

namespace MessageSender.Services.Routing;

public class RoutingViewLocator : IDataTemplate
{
    private readonly ViewRegistry _viewRegistry = new();

    public void RegisterForNavigation<TViewModel, TView>()
        where TViewModel : class, INotifyPropertyChanged
        where TView : Control, new()
    {
        _viewRegistry.Register<TViewModel>(new Activity(typeof(TView), () => new TView()));
    }

    public virtual Control Build(object? data)
    {
        try
        {
            return (Control)_viewRegistry.Create(data!);
        }
        catch (Exception)
        {
            return new TextBlock
            {
                Text = "No view registered for " + data?.GetType().FullName
            };
        }
    }

    public virtual bool Match(object? data)
    {
        return data is INotifyPropertyChanged;
    }
}
