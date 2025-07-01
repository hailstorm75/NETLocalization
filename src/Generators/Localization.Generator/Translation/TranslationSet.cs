namespace Localization.Generator.Translation;

public sealed record TranslationSet(string Key, string Description, IReadOnlyDictionary<string, TranslationItem> Items);