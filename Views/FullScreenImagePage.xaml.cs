using CatDex.ViewModels;

namespace CatDex.Views;

public partial class FullScreenImagePage : ContentPage
{
	public FullScreenImagePage(FullScreenImageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
