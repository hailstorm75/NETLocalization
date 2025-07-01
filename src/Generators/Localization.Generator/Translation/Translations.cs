namespace Localization.Generator.Translation;

public sealed record Translations(string Key, string Description, IReadOnlyDictionary<string, Translation> Items);