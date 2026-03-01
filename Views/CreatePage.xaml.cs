using CatDex.ViewModels;
using CatDex.Models;

namespace CatDex.Views;

public partial class CreatePage : ContentPage
{
	private CreateViewModel ViewModel => (CreateViewModel)BindingContext;

	public CreatePage(CreateViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	private void OnBreedSelected(object sender, EventArgs e)
	{
		var picker = (Picker)sender;
		if (picker.SelectedItem is Breed selectedBreed)
		{
			ViewModel.ToggleBreedCommand.Execute(selectedBreed);
			picker.SelectedItem = null; // Reset picker
			ViewModel.ToggleBreedPickerCommand.Execute(null); // Hide picker
		}
	}
}