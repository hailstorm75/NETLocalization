// See https://aka.ms/new-console-template for more information

using Example.Console;
using Localization.Shared;
using Microsoft.Extensions.Logging.Abstractions;

var translator = new Translator(NullLogger<Translator>.Instance);
translator.RegisterTranslations(Provider.GetTranslations());

CultureManager.Initialize(translator);

translator.ChangeCulture("de-de");
Console.WriteLine(Provider.Farewell);

translator.ChangeCulture("en");
Console.WriteLine(Provider.Farewell);

Console.ReadKey();
