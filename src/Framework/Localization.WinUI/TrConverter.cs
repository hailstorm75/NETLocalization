using Microsoft.UI.Xaml.Data;
using Localization.Shared.Models;

namespace Localization.WinUI;

/// <summary>
/// Converter for localized strings, supports formatting with parameters
/// </summary>
public sealed class TrConverter : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (value is LString lstring)
        {
            if (parameter is object[] args && args.Length > 0)
                return lstring.Format(args);
            if (parameter is not null)
                return lstring.Format(parameter);
            return lstring.String;
        }
        return value?.ToString() ?? string.Empty;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
