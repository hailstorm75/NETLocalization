using Localization.Shared.Models;
using Avalonia.Data.Converters;
using Avalonia.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;

namespace Localization.Avalonia;

internal sealed class TrArgumentConverter : IMultiValueConverter
{
    public required LString Data { get; init; }
    public required IReadOnlyCollection<IBinding> Arguments { get; init; } = [];
    public required IValueConverter? Converter { get; init; }
    public required object? ConverterParameter { get; init; }
    public required CultureInfo ConverterCulture { get; init; }

    /// <inheritdoc />
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var localizationArguments = new List<object>();
        var offset = 1; // The first value is always the localized string

        foreach (var argument in Arguments)
        {
            if (argument is MultiBinding multiBindingArg)
            {
                var bindingsCount = multiBindingArg.Bindings.Count;
                // You may need to store the converter instance for each argument if you use nested MultiBindings
                if (multiBindingArg.Converter is not null)
                {
                    var arg = multiBindingArg.Converter.Convert(
                        values.Skip(offset).Take(bindingsCount).ToList(),
                        targetType,
                        multiBindingArg.ConverterParameter,
                        multiBindingArg.ConverterCulture ?? culture
                    );
                    localizationArguments.Add(arg ?? string.Empty);
                }
                offset += bindingsCount;
            }
            else
            {
                if (values.Count > offset)
                    localizationArguments.Add(values[offset] ?? string.Empty);
                offset++;
            }

        }

        var translated = string.Format(Data.String, localizationArguments.ToArray());

        if (Converter is not null)
            return Converter.Convert(translated, targetType, ConverterParameter, ConverterCulture ?? culture)?.ToString() ?? string.Empty;

        return translated;
    }

    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
