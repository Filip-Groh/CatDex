using CatDex.ViewModels;

namespace CatDex.Views;

public partial class FavoritePage : ContentPage
{
	private readonly FavoriteViewModel _viewModel;
	private bool _isFirstAppearing = true;

	public FavoritePage(FavoriteViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		_viewModel = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		if (_isFirstAppearing)
		{
			_isFirstAppearing = false;
            _ = Task.Run(async () => await _viewModel.InitializeAsync());
		}
		else
		{
            _ = Task.Run(async () => await _viewModel.LoadCatsAsync());
		}
	}
}
