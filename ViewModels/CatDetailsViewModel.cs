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
            }
            finally {
                IsLoading = false;
            }
        }

        [RelayCommand]
        async Task ToggleFavorite() {
            if (Cat == null)
                return;

            Cat.IsFavorite = !Cat.IsFavorite;
            await _repository.SetCatIsFavorite(Cat.Id, Cat.IsFavorite);
            OnPropertyChanged(nameof(Cat));
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
