using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Localization.Shared.Interfaces;
using Localization.Shared.Models;
using System.Collections.ObjectModel;

namespace Example.Avalonia.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private Language _language = "en";
    [ObservableProperty] private Status _status = Status.Draft;

    public ObservableCollection<Language> Languages { get; } = new();

    public MainWindowViewModel()
    {
        var translator = Ioc.Default.GetRequiredService<ITranslator>();
        foreach (var lang in translator.LoadedLanguages)
            Languages.Add(lang);
    }

    partial void OnLanguageChanged(Language value)
        => Ioc.Default.GetRequiredService<ICultureManager>().SetLanguage(value);
}
