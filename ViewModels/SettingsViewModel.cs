using CatDex.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CatDex.ViewModels {
    public partial class SettingsViewModel : ObservableObject {
        private readonly ICatRepositoryService _catRepositoryService;

        [ObservableProperty]
        public partial int TotalCats { get; set; }

        [ObservableProperty]
        public partial int FavoriteCats { get; set; }

        [ObservableProperty]
        public partial int CreatedCats { get; set; }

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        public SettingsViewModel(ICatRepositoryService catRepositoryService) {
            _catRepositoryService = catRepositoryService;

            Task.Run(async () => await LoadStatisticsAsync());
        }

        async Task LoadStatisticsAsync() {
            if (IsBusy)
                return;

            try {
                IsBusy = true;

                var cats = await _catRepositoryService.GetStoredCatsAsync();

                TotalCats = cats.Count;
                FavoriteCats = cats.Count(cat => cat.IsFavorite);
                CreatedCats = cats.Count(cat => cat.StoredImage != null);
            } finally {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task Refresh() {
            await LoadStatisticsAsync();
        }

        [RelayCommand]
        async Task DeleteNonCreatedNonFavorite() {
            if (IsBusy)
                return;

            try {
                bool confirm = await Shell.Current.DisplayAlertAsync(
                    "Delete Cats",
                    "Are you sure you want to delete all non-created and non-favorite cats?",
                    "Delete",
                    "Cancel");

                if (!confirm)
                    return;

                IsBusy = true;

                var deletedCount = await _catRepositoryService.DeleteNonCreatedNonFavoriteCatsAsync();

                await Shell.Current.DisplayAlertAsync(
                    "Success",
                    $"Deleted {deletedCount} cat(s)",
                    "OK");

                await LoadStatisticsAsync();
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error deleting cats: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to delete cats: {ex.Message}", "OK");
            } finally {
                IsBusy = false;
            }
        }
    }
}
