using CatDex.Services.Interfaces;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CatDex.Models.DTOs;
using System.Diagnostics;

namespace CatDex.ViewModels {
    public partial class DiscoveryViewModel : ObservableObject {
        private readonly ICatRepositoryService _repository;
        private readonly IFileSaverService _fileSaverService;
        private int _currentPage = 0;

        public ObservableCollection<CatDTO> Cats { get; } = new();
        public Dictionary<string, bool> StoredCatsFavoriteStatus { get; } = new();

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial CatDTO? SelectedCat { get; set; }

        public DiscoveryViewModel(ICatRepositoryService repository, IFileSaverService fileSaverService) {
            _repository = repository;
            _fileSaverService = fileSaverService;

            Task.Run(async () => await OnThresholdReached());
        }

        [RelayCommand]
        async Task OnThresholdReached() {
            if (IsBusy)
                return;

            try {
                IsBusy = true;

                var newCats = await _repository.GetNewCatsAsync(page: _currentPage, limit: 10);

                if (newCats.Count > 0) {
                    var storedCats = await _repository.GetStoredCatsAsync();

                    foreach (var storedCat in storedCats) {
                        StoredCatsFavoriteStatus[storedCat.Id] = storedCat.IsFavorite;
                    }

                    foreach (var cat in newCats) {
                        Cats.Add(cat);
                    }

                    _currentPage++;
                }
            } finally {
                IsBusy = false;
            }
        }

        public async Task OnCatSelected() {
            Debug.WriteLine(SelectedCat?.Url);
            if (SelectedCat != null) {
                var storedCat = await _repository.StoreCatAsync(SelectedCat.Id);
                StoredCatsFavoriteStatus[storedCat.Id] = storedCat.IsFavorite;
                OnPropertyChanged(nameof(StoredCatsFavoriteStatus));
            }
        }

        public bool IsCatStored(string catId) {
            return StoredCatsFavoriteStatus.ContainsKey(catId);
        }

        public bool GetCatFavoriteStatus(string catId) {
            return StoredCatsFavoriteStatus.TryGetValue(catId, out var isFavorite) && isFavorite;
        }

        [RelayCommand]
        async Task ToggleFavorite(CatDTO cat) {
            if (cat == null || !IsCatStored(cat.Id))
                return;

            try {
                var currentStatus = GetCatFavoriteStatus(cat.Id);
                var newIsFavorite = !currentStatus;
                await _repository.SetCatIsFavorite(cat.Id, newIsFavorite);

                StoredCatsFavoriteStatus[cat.Id] = newIsFavorite;
                OnPropertyChanged(nameof(StoredCatsFavoriteStatus));
            } catch (Exception ex) {
                Debug.WriteLine($"Error toggling favorite: {ex.Message}");
            }
        }

        [RelayCommand]
        async Task NavigateToCatDetails(CatDTO cat) {
            if (cat == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(Views.CatDetailsPage)}?CatId={cat.Id}");
        }

        [RelayCommand]
        async Task DownloadImage(CatDTO cat) {
            if (cat == null)
                return;

            try {
                var fileName = $"cat_{cat.Id}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                var success = await _fileSaverService.SaveImageAsync(cat.Url, null, fileName);

                if (success) {
                    await Shell.Current.DisplayAlert("Success", "Image saved successfully!", "OK");
                } else {
                    await Shell.Current.DisplayAlert("Error", "Failed to save image.", "OK");
                }
            } catch (Exception ex) {
                Debug.WriteLine($"Error downloading image: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to save image.", "OK");
            }
        }
    }
}
