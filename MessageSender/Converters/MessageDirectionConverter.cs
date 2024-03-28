using System;
using System.Globalization;
using Avalonia.Data.Converters;
using MessageSender.Services.Interaction;

namespace MessageSender.Converters;

public class MessageDirectionConverter: IValueConverter
{
    public static MessageDirectionConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string result = (MessageDirection)value! switch
        {
            MessageDirection.In => "<-",
            MessageDirection.Out => "->",
            _ => throw new ArgumentOutOfRangeException()
        };

        return result;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}