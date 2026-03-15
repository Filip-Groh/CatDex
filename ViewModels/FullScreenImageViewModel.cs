using CatDex.Constants;
using CatDex.Models;
using CatDex.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CatDex.ViewModels;

[QueryProperty(nameof(ImageUrl), AppConstants.QueryParameters.ImageUrl)]
[QueryProperty(nameof(ImageData), AppConstants.QueryParameters.ImageData)]
public partial class FullScreenImageViewModel(IFileSaverService fileSaverService, INavigationService navigationService, IDialogService dialogService) : ObservableObject
{
    [ObservableProperty]
    public partial string? ImageUrl { get; set; }

    [ObservableProperty]
    public partial ImageData? ImageData { get; set; }

    [RelayCommand]
	private async Task Close()
	{
		await navigationService.GoBackAsync();
	}

	[RelayCommand]
	private async Task Download()
	{
		var fileName = string.Format(AppConstants.Files.ImageFileNameFormat, DateTime.Now);
		var success = await fileSaverService.SaveImageAsync(ImageUrl, ImageData?.Bytes, fileName);

		if (success)
		{
			await dialogService.ShowAlertAsync("Success", "Image saved successfully!", "OK");
		}
		else
		{
			await dialogService.ShowAlertAsync("Error", "Failed to save image.", "OK");
		}
	}
}
