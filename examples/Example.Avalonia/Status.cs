using Localization.Shared.Attributes;

namespace Example.Avalonia;

[LocalizedEnum<Provider>] // Required decoration to make an enum localizable
public enum Status
{
    // Localization retrieved automatically from parent `LocalizedEnum<Provider>` attribute
    Draft,

    // Specific localization from type-safe field
    [LocalizedEnumField(nameof(Provider.PublishedStatus))]
    Published,

    // Another way to specify the specific localization using type-safe field access
    [LocalizedEnumField(typeof(Provider), nameof(Provider.Archived))]
    Archived
}
