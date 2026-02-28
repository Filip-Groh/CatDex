using CatDex.ViewModels;

namespace CatDex.Views;

public partial class CreatePage : ContentPage
{
	public CreatePage(CreateViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}