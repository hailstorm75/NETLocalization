using Microsoft.Extensions.Localization;
using Localization.Shared.Interfaces;
using Localization.Shared.Models;

namespace Localization.StringLocalizer;

public sealed class LStringLocalizer(ITranslator translator) : IStringLocalizer
{
    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => translator.GetAllTranslations().Select(Convert);

    /// <inheritdoc />
    public LocalizedString this[string name]
    {
        get
        {
            var split = LString.SplitIdentifier(name);
            var localized = translator.Translate(split.key, split.@namespace);

            return new LocalizedString(name, localized);
        }
    }

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var split = LString.SplitIdentifier(name);
            var localized = translator.TranslateArgs(split.key, split.@namespace, arguments);

            return new LocalizedString(name, localized);
        }
    }

    private static LocalizedString Convert(LString str) => new(str.Identifier, str);
}
