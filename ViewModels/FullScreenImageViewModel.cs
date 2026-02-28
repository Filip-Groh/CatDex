using CatDex.Models;
using CatDex.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CatDex.ViewModels;

[QueryProperty(nameof(ImageUrl), nameof(ImageUrl))]
[QueryProperty(nameof(ImageData), nameof(ImageData))]
public partial class FullScreenImageViewModel : ObservableObject
{
	private readonly IFileSaverService _fileSaverService;

	[ObservableProperty]
	private string? imageUrl;

	[ObservableProperty]
	private ImageData? imageData;

	public FullScreenImageViewModel(IFileSaverService fileSaverService)
	{
		_fileSaverService = fileSaverService;
	}

	[RelayCommand]
	private async Task Close()
	{
		await Shell.Current.GoToAsync("..");
	}

	[RelayCommand]
	private async Task Download()
	{
		var fileName = $"cat_{DateTime.Now:yyyyMMddHHmmss}.jpg";
		var success = await _fileSaverService.SaveImageAsync(ImageUrl, ImageData?.Bytes, fileName);

		if (success)
		{
			await Shell.Current.DisplayAlert("Success", "Image saved successfully!", "OK");
		}
		else
		{
			await Shell.Current.DisplayAlert("Error", "Failed to save image.", "OK");
		}
	}
}
