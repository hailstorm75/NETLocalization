using System.Collections;

namespace Localization.Generator.Translation;

public sealed class TranslationsCollection : IEnumerable<TranslationSet>
{
    private readonly Dictionary<string, TranslationSet> _sets;

    public string Namespace { get; }

    public TranslationsCollection(string @namespace, IEnumerable<TranslationSet> sets)
    {
        Namespace = @namespace;
        _sets = sets.ToDictionary(static set => set.Key, static set => set, StringComparer.OrdinalIgnoreCase);
    }

    public TranslationSet this[string key]
        => _sets[key];

    /// <inheritdoc />
    public IEnumerator<TranslationSet> GetEnumerator()
        => _sets.Values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}