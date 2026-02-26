using CatDex.ViewModels;

namespace CatDex.Views;

public partial class DiscoverPage : ContentPage
{
	public DiscoverPage(CatViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}