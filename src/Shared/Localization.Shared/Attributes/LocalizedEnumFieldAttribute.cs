namespace Localization.Shared.Attributes;

/// <summary>
/// Attribute to localize an enum field with a specific translation key.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class LocalizedEnumFieldAttribute : Attribute
{
    /// <summary>
    /// Provider type that owns the strongly typed translation key.
    /// </summary>
    public Type? ProviderType { get; }

    /// <summary>
    /// Key of the translation.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Initializes a localized enum field with an explicit translation key.
    /// </summary>
    /// <param name="key">Translation key.</param>
    public LocalizedEnumFieldAttribute(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Initializes a localized enum field with a provider-scoped translation key.
    /// </summary>
    /// <param name="providerType">Generated translation provider type.</param>
    /// <param name="key">Translation key.</param>
    public LocalizedEnumFieldAttribute(Type providerType, string key)
    {
        ProviderType = providerType;
        Key = key;
    }
}
