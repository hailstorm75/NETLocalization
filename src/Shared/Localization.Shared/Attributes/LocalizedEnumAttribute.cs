namespace Localization.Shared.Attributes;

/// <summary>
/// Attribute for marking an enum as localized.
/// </summary>
[AttributeUsage(AttributeTargets.Enum, Inherited = false)]
public sealed class LocalizedEnumAttribute : Attribute
{
    /// <summary>
    /// Namespace of the translations used by this enum.
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// Initializes a localized enum with its translation namespace.
    /// </summary>
    /// <param name="namespace">Translation namespace.</param>
    public LocalizedEnumAttribute(string @namespace)
    {
        Namespace = @namespace;
    }
}
