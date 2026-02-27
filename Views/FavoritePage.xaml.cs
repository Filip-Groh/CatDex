using CatDex.ViewModels;

namespace CatDex.Views;

public partial class FavoritePage : ContentPage
{
	public FavoritePage(FavoriteViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}