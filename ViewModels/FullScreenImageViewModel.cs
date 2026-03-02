using CatDex.Constants;
using CatDex.Models;
using CatDex.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CatDex.ViewModels;

[QueryProperty(nameof(ImageUrl), AppConstants.QueryParameters.ImageUrl)]
[QueryProperty(nameof(ImageData), AppConstants.QueryParameters.ImageData)]
public partial class FullScreenImageViewModel : ObservableObject
{
	private readonly IFileSaverService _fileSaverService;
	private readonly INavigationService _navigationService;
	private readonly IDialogService _dialogService;

	[ObservableProperty]
	private string? imageUrl;

	[ObservableProperty]
	private ImageData? imageData;

	public FullScreenImageViewModel(IFileSaverService fileSaverService, INavigationService navigationService, IDialogService dialogService)
	{
		_fileSaverService = fileSaverService;
		_navigationService = navigationService;
		_dialogService = dialogService;
	}

	[RelayCommand]
	private async Task Close()
	{
		await _navigationService.GoBackAsync();
	}

	[RelayCommand]
	private async Task Download()
	{
		var fileName = string.Format(AppConstants.Files.ImageFileNameFormat, DateTime.Now);
		var success = await _fileSaverService.SaveImageAsync(ImageUrl, ImageData?.Bytes, fileName);

		if (success)
		{
			await _dialogService.ShowAlertAsync("Success", "Image saved successfully!", "OK");
		}
		else
		{
			await _dialogService.ShowAlertAsync("Error", "Failed to save image.", "OK");
		}
	}
}
