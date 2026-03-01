using CatDex.Models;
using CatDex.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CatDex.ViewModels {
    [QueryProperty(nameof(CatId), nameof(CatId))]
    public partial class CatDetailsViewModel : ObservableObject {
        private readonly ICatRepositoryService _repository;

        [ObservableProperty]
        public partial string? CatId { get; set; }

        [ObservableProperty]
        public partial Cat? Cat { get; set; }

        [ObservableProperty]
        public partial bool IsLoading { get; set; }

        public CatDetailsViewModel(ICatRepositoryService repository) {
            _repository = repository;
        }

        partial void OnCatIdChanged(string? value) {
            if (!string.IsNullOrEmpty(value)) {
                Task.Run(async () => await LoadCatAsync());
            }
        }

        async Task LoadCatAsync() {
            if (string.IsNullOrEmpty(CatId))
                return;

            IsLoading = true;

            try {
                Cat = await _repository.GetCatByIdAsync(CatId);
            } catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase)) {
                System.Diagnostics.Debug.WriteLine($"Rate limit hit when loading cat {CatId}.");
                await Shell.Current.DisplayAlert("Rate Limit", "API rate limit exceeded. Please try again later.", "OK");
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error loading cat {CatId}: {ex.Message}");
            } finally {
                IsLoading = false;
            }
        }

        [RelayCommand]
        async Task ToggleFavorite() {
            if (Cat == null)
                return;

            try {
                Cat.IsFavorite = !Cat.IsFavorite;
                await _repository.SetCatIsFavorite(Cat.Id, Cat.IsFavorite);
                OnPropertyChanged(nameof(Cat));
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error toggling favorite: {ex.Message}");
                Cat.IsFavorite = !Cat.IsFavorite; // Revert on error
            }
        }

        [RelayCommand]
        async Task Delete() {
            if (Cat == null)
                return;

            try {
                bool confirm = await Shell.Current.DisplayAlertAsync(
                    "Delete Cat",
                    $"Are you sure you want to delete this cat (ID: {Cat.Id})?",
                    "Delete",
                    "Cancel");

                if (!confirm)
                    return;

                await _repository.DeleteCatAsync(Cat.Id);
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex) {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to delete cat: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        async Task GoBack() {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task OpenUrl(string url) {
            if (string.IsNullOrWhiteSpace(url))
                return;

            try {
                await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex) {
                // Handle exception if browser fails to open
                System.Diagnostics.Debug.WriteLine($"Failed to open URL: {ex.Message}");
            }
        }
    }
}
