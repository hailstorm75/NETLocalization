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
    /// Determines whether reflection is allowed to be used for translation generation
    /// </summary>
    public bool AllowReflection { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="source">Translation file source</param>
    /// <param name="allowReflection">Disable for AOT support</param>
    public TranslationProviderAttribute(string source, bool allowReflection = false)
    {
        Source = source;
        AllowReflection = allowReflection;
    }
}