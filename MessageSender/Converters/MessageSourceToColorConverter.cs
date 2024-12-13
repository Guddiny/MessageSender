using ActiproSoftware.UI.Avalonia.Themes;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using MessageSender.Services.Interaction;
using System;
using System.Globalization;

namespace MessageSender.Converters;

public class MessageSourceToColorConverter : IValueConverter
{
    public static MessageSourceToColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        IBrush cloudColor;
        IBrush messageSenderColor;
        IBrush simulatorColor;

        if (Application.Current!.ActualThemeVariant.IsDark())
        {
            cloudColor = Brush.Parse("#1d403e");
            messageSenderColor = Brush.Parse("#571c5c");
            simulatorColor = Brush.Parse("#952354");
        }
        else
        {
            cloudColor = Brush.Parse("#e5e6e8");
            messageSenderColor = Brush.Parse("#fef6eb");
            simulatorColor = Brush.Parse("#f8caca");
        }

        IBrush? color = (MessageSource)value! switch
        {
            MessageSource.Cloud => cloudColor,
            MessageSource.MessageSender => messageSenderColor,
            MessageSource.Simulator => simulatorColor,
            _ => throw new ArgumentOutOfRangeException()
        };

        return color;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}