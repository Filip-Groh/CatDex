using CatDex.ViewModels;

namespace CatDex.Views;

public partial class CatDetailsPage : ContentPage
{
    public CatDetailsPage(CatDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
