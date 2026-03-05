using CatDex.ViewModels;

namespace CatDex.Views;

public partial class SettingsPage : ContentPage
{
	private readonly SettingsViewModel _viewModel;
	private bool _isFirstAppearing = true;

	public SettingsPage(SettingsViewModel viewModel)
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
		}
		else
		{
			await _viewModel.RefreshCommand.ExecuteAsync(null);
		}
	}
}
