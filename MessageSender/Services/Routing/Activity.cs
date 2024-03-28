using System;

namespace MessageSender.Services.Routing;

public class Activity
{
    public Type ViewType { get; }

    public Func<object> CreateFunc { get; }

    public Activity(Type viewType, Func<object> createFunc)
    {
        CreateFunc = createFunc;
        ViewType = viewType;
    }

    public object Create()
    {
        return CreateFunc();
    }

    public bool TypeDerivesFrom<T>()
    {
        return typeof(T).IsAssignableFrom(ViewType);
    }
}
