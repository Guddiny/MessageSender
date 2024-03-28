using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using MessageSender.Services.Interaction;

namespace MessageSender.Converters;

public class MessageSourceToColorConverter : IValueConverter
{
    public static MessageSourceToColorConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        IBrush color = (MessageSource)value! switch
        {
            MessageSource.Cloud => Brush.Parse("#e5e6e8"),
            MessageSource.MessageSender => Brush.Parse("#fef6eb"),
            MessageSource.Simulator => Brush.Parse("#f8caca"),
            _ => throw new ArgumentOutOfRangeException()
        };

        return color;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}