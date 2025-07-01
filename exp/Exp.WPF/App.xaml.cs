using CommunityToolkit.Mvvm.DependencyInjection;
using Localization.Shared.Interfaces;
using Localization.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace Exp.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App
{
    public App()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddSingleton<ITranslator, Translator>()
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());

        CultureManager.Initialize(Ioc.Default);

        var translator = Ioc.Default.GetRequiredService<ITranslator>();
        translator.RegisterTranslations(Provider.GetTranslations());
    }
}
