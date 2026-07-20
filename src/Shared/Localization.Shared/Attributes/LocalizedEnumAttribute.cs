using Localization.Shared.Interfaces;

namespace Localization.Shared.Attributes;

/// <summary>
/// Attribute for marking an enum as localized.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public sealed class LocalizedEnumAttribute<T> : LocalizedEnumAttribute where T : ITranslationProvider
{
    /// <summary>
    /// Initializes a localized enum with its translation namespace.
    /// </summary>
    public LocalizedEnumAttribute() : base(T.GetNamespace())
    {
    }
}

/// <summary>
/// Attribute for marking an enum as localized.
/// </summary>
public abstract class LocalizedEnumAttribute(string @namespace) : Attribute
{
    /// <summary>
    /// Namespace of the translations used by this enum.
    /// </summary>
    public string Namespace { get; } = @namespace;
}
