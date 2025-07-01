using System.Collections;

namespace Localization.Generator.Translation;

public sealed class TranslationsCollection : IEnumerable<Translations>
{
    private readonly Dictionary<string, Translations> _sets;

    /// <summary>
    /// Translation namespace
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// Items count in the collection
    /// </summary>
    public int Count => _sets.Count;

    public TranslationsCollection(string @namespace, IEnumerable<Translations> sets)
    {
        Namespace = @namespace;
        _sets = sets.ToDictionary(static set => set.Key, static set => set, StringComparer.OrdinalIgnoreCase);
    }

    public Translations this[string key]
        => _sets[key];

    /// <inheritdoc />
    public IEnumerator<Translations> GetEnumerator()
        => _sets.Values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}