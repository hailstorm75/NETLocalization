using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Localization.Shared;
using Localization.Shared.Interfaces;
using Localization.Shared.Models;

namespace Exp.WPF;

public sealed partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private Language _language = "en";

    public ObservableCollection<Language> Languages { get; } = new();

    public MainWindowViewModel()
    {
        var translator = Ioc.Default.GetRequiredService<ITranslator>();
        foreach (var lang in translator.LoadedLanguages)
            Languages.Add(lang);
    }

    partial void OnLanguageChanged(Language value) => CultureManager.SetLanguage(value);
}
