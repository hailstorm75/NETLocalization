using CommunityToolkit.Mvvm.DependencyInjection;
using Localization.Shared.DependencyInjection;
using Localization.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Example.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App
{
    public App()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddLocalization(_ => { })
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());

        var translator = Ioc.Default.GetRequiredService<ITranslator>();
        translator.LoadTranslations();
    }
}
