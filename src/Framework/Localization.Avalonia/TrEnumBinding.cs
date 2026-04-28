using System;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace Localization.Avalonia;

/// <summary>
/// Provides localized enum values for Avalonia item controls.
/// </summary>
public sealed class TrEnumBinding : MarkupExtension
{
    private readonly Type? _enumType;

    /// <summary>
    /// Initializes a new enum binding.
    /// </summary>
    /// <param name="enumType">Enum type.</param>
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
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (EnumType is null)
            return Array.Empty<object>();

        return EnumType.GetLocalizedValues();
    }
}
