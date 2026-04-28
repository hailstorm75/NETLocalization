// See https://aka.ms/new-console-template for more information

using Example.Console;
using Localization.Shared.DependencyInjection;
using Localization.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

var services = new ServiceCollection()
    .AddLocalization(_ => { })
    .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
    .BuildServiceProvider();

var translator = services.GetRequiredService<ITranslator>();
translator.RegisterTranslations(Provider.GetTranslations());

var cultureManager = services.GetRequiredService<ICultureManager>();
cultureManager.SetLanguage("de-de");
Console.WriteLine(Provider.Farewell);

cultureManager.SetLanguage("en");
Console.WriteLine(Provider.Farewell);

Console.ReadKey();
