using System.Globalization;

namespace Localization.MAUI;

/// <summary>
/// Converter for formatting localized strings with arguments
/// </summary>
public sealed class TrFormatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str)
            return value;

        if (parameter is object[] args && args.Length > 0)
            return string.Format(str, args.Select(static a => a.ToString() as object).ToArray());

        return str;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
