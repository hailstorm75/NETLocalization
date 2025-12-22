namespace Localization.Shared.Interfaces;

/// <summary>
/// Defines a contract for a translation provider that supplies localization data.
/// </summary>
public interface ITranslationProvider
{
    /// <summary>
    /// Returns the namespace of the localization provider.
    /// This method is intended to provide a unique identifier or grouping
    /// for the localization resources managed by the provider.
    /// </summary>
    /// <returns>A string representing the namespace of the localization provider.</returns>
    static abstract string GetNamespace();
}
