namespace Localization.Generator.Translation;

internal sealed record Translations(string Key, string Description, IReadOnlyDictionary<string, Translation> Items);
