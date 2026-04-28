using Localization.Shared.Interfaces;
using System.Threading;

namespace Localization.Shared;

internal static class LocalizationAmbient
{
    private static ICultureManager? _cultureManager;
    private static ITranslator? _translator;

    internal static bool TryGetCultureManager(out ICultureManager? cultureManager)
    {
        cultureManager = Volatile.Read(ref _cultureManager);
        return cultureManager is not null;
    }

    internal static bool TryGetTranslator(out ITranslator? translator)
    {
        translator = Volatile.Read(ref _translator);
        return translator is not null;
    }

    internal static ICultureManager GetRequiredCultureManager()
        => Volatile.Read(ref _cultureManager)
           ?? throw new InvalidOperationException("Localization is not initialized. Resolve localization services via AddLocalization and ITranslator.");

    internal static void Register(ICultureManager cultureManager)
    {
        ArgumentNullException.ThrowIfNull(cultureManager);
        Volatile.Write(ref _cultureManager, cultureManager);
    }

    internal static void Register(ITranslator translator)
    {
        ArgumentNullException.ThrowIfNull(translator);
        Volatile.Write(ref _translator, translator);
    }
}
