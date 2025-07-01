namespace Localization.Shared.Attributes;

/// <summary>
/// Attribute for marking a class as a target for translation generation
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class TranslationProviderAttribute : Attribute
{
    /// <summary>
    /// Translation file name
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="source">Translation file source</param>
    public TranslationProviderAttribute(string source)
    {
        Source = source;
    }
}