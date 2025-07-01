using System.Collections;

namespace Localization.Generator.Translation;

public sealed class TranslationsCollection : IEnumerable<Translations>, IEquatable<TranslationsCollection>
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

    /// <inheritdoc />
    public bool Equals(TranslationsCollection? other)
    {
        if (other is null)
            return false;

        if (!Namespace.Equals(other.Namespace, StringComparison.Ordinal))
            return false;

        if (Count != other.Count)
            return false;

        foreach (var set in _sets)
            if (!other._sets.TryGetValue(set.Key, out var otherValue) || !set.Value.Equals(otherValue))
                return false;

        return true;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is TranslationsCollection other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (_sets.GetHashCode() * 397) ^ Namespace.GetHashCode();
        }
    }

    public static bool operator ==(TranslationsCollection? left, TranslationsCollection? right) => Equals(left, right);

    public static bool operator !=(TranslationsCollection? left, TranslationsCollection? right) => !Equals(left, right);
}
