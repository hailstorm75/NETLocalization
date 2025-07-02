using Exp.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Exp.WinUI.Views;

public sealed partial class MainPageViewPage : Page
{
    public MainPageViewViewModel ViewModel
    {
        get;
    }

    public MainPageViewPage()
    {
        ViewModel = App.GetService<MainPageViewViewModel>();
        InitializeComponent();
    }
}
