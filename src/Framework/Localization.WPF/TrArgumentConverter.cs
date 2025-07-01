using System.Globalization;
using System.Windows.Data;
using Localization.Shared.Models;

namespace Localization.WPF;

internal sealed class TrArgumentConverter
    : IMultiValueConverter
{
    public required LString Data { get; init; }
    public required IReadOnlyCollection<BindingBase> Arguments { get; init; } = [];
    public required IValueConverter? Converter { get; init; }
    public required object? ConverterParameter { get; init; }
    public required CultureInfo ConverterCulture { get; init; }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var localizationArguments = new List<object>();

        // Not 0, because the converted multibinding contains the LocString as the first binding
        // This is to invoke the converter to provide a new string if the language changes
        var offset = 1;
        foreach (var argument in Arguments)
        {
            if (argument is MultiBinding argumentMultiBinding)
            {
                var bindingsCount = argumentMultiBinding.Bindings.Count;

                var arg = argumentMultiBinding.Converter
                    .Convert(
                        values.Skip(offset).Take(bindingsCount).ToArray(),
                        null,
                        argumentMultiBinding.ConverterParameter,
                        argumentMultiBinding.ConverterCulture);
                localizationArguments.Add(arg);
                offset += bindingsCount;
            }
            else
            {
                if (values.Length > offset)
                    localizationArguments.Add(values[offset]);
                offset++;
            }
        }

        var translated = string.Format(Data.String, localizationArguments.ToArray());

        if (Converter is not null)
            return Converter!.Convert(translated, targetType, ConverterParameter, ConverterCulture)?.ToString() ?? string.Empty;

        return translated;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}