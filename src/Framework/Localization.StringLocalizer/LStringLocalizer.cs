using Microsoft.Extensions.Localization;
using Localization.Shared.Interfaces;
using Localization.Shared.Models;

namespace Localization.StringLocalizer;

public sealed class LStringLocalizer<T>(ITranslator translator) : IStringLocalizer<T> where T : ITranslationProvider
{
    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => translator.GetAllTranslations().Select(Convert);

    /// <inheritdoc />
    public LocalizedString this[string name]
    {
        get
        {
            var localized = translator.Translate(name, T.GetNamespace());

            return new LocalizedString(name, localized);
        }
    }

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var localized = translator.TranslateArgs(name, T.GetNamespace(), arguments);

            return new LocalizedString(name, localized);
        }
    }

    private static LocalizedString Convert(LString str) => new(str.Identifier, str);
}
