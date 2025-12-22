using Localization.Shared.Attributes;

namespace Example.WPF;

[TranslationProvider("UIStrings")]
public sealed partial class Provider;

[TranslationProviderAggregate]
public static partial class TranslationsAggregate;
