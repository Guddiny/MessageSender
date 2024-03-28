using MessageSender.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MessageSender.Services.Routing;

public static class RouterExtensions
{
    public static IServiceCollection AddRouting(
        this IServiceCollection serviceCollection,
        Action<RoutingViewLocator> locator)
    {
        var locatorInstance = new RoutingViewLocator();

        locator(locatorInstance);

        serviceCollection.AddSingleton(locatorInstance);
        serviceCollection.AddSingleton(s =>
            new HistoryRouter<ViewModelBase>(t => (ViewModelBase)s.GetRequiredService(t)));

        return serviceCollection;
    }
}
