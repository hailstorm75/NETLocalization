namespace Localization.Shared.Attributes;

/// <summary>
/// Attribute for marking a class as a target for generating an aggregate of all translation providers.
/// </summary>
/// <seealso cref="TranslationProviderAttribute"/>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class TranslationProviderAggregateAttribute : Attribute;
