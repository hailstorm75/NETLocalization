using Localization.Shared.Models;
using System.Globalization;

namespace Localization.MAUI;

/// <summary>
/// Converter for formatting localized strings with arguments
/// </summary>
public sealed class TrFormatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not object[] args || args.Length <= 0)
            return value;

        return value switch
        {
            string str => string.Format(str, args.Select(static a => a.ToString() as object).ToArray()),
            LString localizedString => localizedString.Format(args),
            _ => value
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
