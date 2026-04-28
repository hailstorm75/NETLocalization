using Localization.Shared.Attributes;

namespace Example.WPF;

[LocalizedEnum<Provider>]
public enum Status
{
    Draft,

    [LocalizedEnumField(nameof(Provider.PublishedStatus))]
    Published,

    [LocalizedEnumField(typeof(Provider), nameof(Provider.Archived))]
    Archived
}
