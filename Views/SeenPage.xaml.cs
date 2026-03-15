using CatDex.ViewModels;

namespace CatDex.Views;

public partial class SeenPage : ContentPage
{
	private readonly SeenViewModel _viewModel;
	private bool _isFirstAppearing = true;

	public SeenPage(SeenViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		_viewModel = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

        if (_isFirstAppearing) {
            _isFirstAppearing = false;
            _ = Task.Run(async () => await _viewModel.InitializeAsync());
        } else {
            _ = Task.Run(async () => await _viewModel.LoadCatsAsync());
        }
    }
}
