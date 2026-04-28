using Localization.Shared.Attributes;

namespace Example.Avalonia;

[LocalizedEnum("Test")]
public enum Status
{
    Draft,

    [LocalizedEnumField(nameof(Provider.PublishedStatus))]
    Published,

    [LocalizedEnumField(typeof(Provider), nameof(Provider.Archived))]
    Archived
}
