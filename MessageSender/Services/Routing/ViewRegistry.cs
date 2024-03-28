using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MessageSender.Services.Routing;

public class ViewRegistry
{
    public readonly Dictionary<Type, Activity> Registrations = [];

    public void Register<TViewModel>(Activity viewDef)
        where TViewModel : INotifyPropertyChanged
    {
        Registrations.Add(typeof(TViewModel), viewDef);
    }

    private Activity Locate(object viewModel)
    {
        if (Registrations.TryGetValue(viewModel.GetType(), out var value))
        {
            return value;
        }

        throw new TypeLoadException(string.Concat("No view was registered for view model " + viewModel.GetType().FullName + ".", Environment.NewLine, "This project uses a ViewRegistry, which requires manually registering all ViewModel-View combinations."));
    }

    public virtual object Create(object viewModel)
    {
        return Locate(viewModel).Create();
    }
}