using CatDex.ViewModels;

namespace CatDex.Views;

public partial class SeenPage : ContentPage
{
	public SeenPage(SeenViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
