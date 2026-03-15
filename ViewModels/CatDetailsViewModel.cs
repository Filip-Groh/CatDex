using CatDex.Constants;
using CatDex.Models;
using CatDex.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CatDex.ViewModels {
    [QueryProperty(nameof(CatId), AppConstants.QueryParameters.CatId)]
    public partial class CatDetailsViewModel(ICatRepositoryService repository, INavigationService navigationService, IDialogService dialogService) : ObservableObject {
        [ObservableProperty]
        public partial string? CatId { get; set; }

        [ObservableProperty]
        public partial Cat? Cat { get; set; }

        [ObservableProperty]
        public partial bool IsLoading { get; set; }

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
                Cat = await repository.GetCatByIdAsync(CatId);
            } catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase)) {
                System.Diagnostics.Debug.WriteLine($"Rate limit hit when loading cat {CatId}.");
                await dialogService.ShowAlertAsync("Rate Limit", "API rate limit exceeded. Please try again later.", "OK");
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
                await repository.SetCatIsFavorite(Cat.Id, Cat.IsFavorite);
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
                bool confirm = await dialogService.ShowConfirmationAsync(
                    "Delete Cat",
                    $"Are you sure you want to delete this cat (ID: {Cat.Id})?",
                    "Delete",
                    "Cancel");

                if (!confirm)
                    return;

                await repository.DeleteCatAsync(Cat.Id);
                await navigationService.GoBackAsync();
            }
            catch (Exception ex) {
                await dialogService.ShowAlertAsync("Error", $"Failed to delete cat: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        async Task GoBack() {
            await navigationService.GoBackAsync();
        }

        [RelayCommand]
        static async Task OpenUrl(string url) {
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
