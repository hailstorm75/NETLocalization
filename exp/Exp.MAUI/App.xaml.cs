using CommunityToolkit.Mvvm.DependencyInjection;
using Localization.Shared;
using Localization.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Exp.MAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddSingleton<ITranslator, Translator>()
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());

        CultureManager.Initialize(Ioc.Default);

        var translator = Ioc.Default.GetRequiredService<ITranslator>();
        translator.RegisterTranslations(Provider.GetTranslations());

        MainPage = new AppShell();
    }
}
