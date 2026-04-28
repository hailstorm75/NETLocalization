using Localization.Shared;
using System.Windows.Data;
using System.Windows.Markup;

namespace Localization.WPF;

/// <summary>
/// Provides localized enum values for WPF item controls.
/// </summary>
public sealed class TrEnumBinding : MarkupExtension
{
    private readonly Type? _enumType;

    /// <summary>
    /// Initializes a new enum binding.
    /// </summary>
    /// <param name="enumType">Enum type or binding with an enum type source.</param>
    public TrEnumBinding(object enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);

        EnumType = enumType switch
        {
            Type type => type,
            Binding binding when binding.Source is Type type => type,
            _ => throw new ArgumentException("Argument must be an enum Type or a Binding with an enum Type source.", nameof(enumType))
        };
    }

    /// <summary>
    /// Enum type to enumerate.
    /// </summary>
    [ConstructorArgument("enumType")]
    public Type? EnumType
    {
        get => _enumType;
        private init
        {
            if (value is null)
            {
                _enumType = null;
                return;
            }

            var enumType = Nullable.GetUnderlyingType(value) ?? value;
            if (!enumType.IsEnum)
                throw new ArgumentException("Type must be an enum.", nameof(value));

            _enumType = enumType;
        }
    }

    /// <inheritdoc />
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        if (EnumType is null)
            return null;

        var translator = LocalizationRuntime.GetTranslator();
        if (translator is null)
            return Array.Empty<object>();

        return Enum.GetValues(EnumType)
            .OfType<object>()
            .Select(translator.GetEnum)
            .ToArray();
    }
}
