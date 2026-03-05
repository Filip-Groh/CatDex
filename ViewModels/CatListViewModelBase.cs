using CatDex.Constants;
using CatDex.Models;
using CatDex.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CatDex.ViewModels {
    public abstract partial class CatListViewModelBase : ObservableObject {
        protected readonly ICatRepositoryService _catRepositoryService;
        protected readonly INavigationService _navigationService;
        protected readonly IDialogService _dialogService;

        public ObservableCollection<Cat> Cats { get; } = new();

        public ObservableCollection<Breed> Breeds { get; } = new();

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial bool IsRefreshing { get; set; }

        [ObservableProperty]
        public partial bool IsListMode { get; set; }

        [ObservableProperty]
        public partial Breed? SelectedBreed { get; set; }

        protected CatListViewModelBase(ICatRepositoryService catRepositoryService, INavigationService navigationService, IDialogService dialogService) {
            _catRepositoryService = catRepositoryService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            Task.Run(async () => {
                await LoadBreedsAsync();
                await LoadCatsAsync();
            });
        }

        protected abstract Task<IEnumerable<Cat>> GetCatsAsync(string? breedId);

        protected async Task LoadBreedsAsync() {
            try {
                var breeds = await _catRepositoryService.GetBreedsAsync();

                Breeds.Clear();
                Breeds.Add(new Breed { Id = "", Name = "All Breeds" });
                Breeds.Add(new Breed { Id = "none", Name = "No Breed" });
                foreach (var breed in breeds.OrderBy(b => b.Name)) {
                    Breeds.Add(breed);
                }

                SelectedBreed = Breeds.FirstOrDefault();
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error loading breeds: {ex.Message}");
            }
        }

        protected async Task LoadCatsAsync() {
            if (IsBusy)
                return;

            try {
                IsBusy = true;

                IEnumerable<Cat> cats;

                if (SelectedBreed?.Id == "none") {
                    var allCats = await GetCatsAsync(null);
                    cats = allCats.Where(c => c.Breeds == null || !c.Breeds.Any());
                } else {
                    var breedId = SelectedBreed?.Id == "" ? null : SelectedBreed?.Id;
                    cats = await GetCatsAsync(breedId);
                }

                Cats.Clear();
                foreach (var cat in cats) {
                    Cats.Add(cat);
                }
            } finally {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OnRefresh() {
            try {
                IsBusy = true;
                IsRefreshing = true;

                IEnumerable<Cat> cats;

                if (SelectedBreed?.Id == "none") {
                    var allCats = await GetCatsAsync(null);
                    cats = allCats.Where(c => c.Breeds == null || !c.Breeds.Any());
                } else {
                    var breedId = SelectedBreed?.Id == "" ? null : SelectedBreed?.Id;
                    cats = await GetCatsAsync(breedId);
                }

                Cats.Clear();
                foreach (var cat in cats) {
                    Cats.Add(cat);
                }
            } finally {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        async Task ToggleFavorite(Cat cat) {
            if (cat == null)
                return;

            try {
                var newIsFavorite = !cat.IsFavorite;
                await _catRepositoryService.SetCatIsFavorite(cat.Id, newIsFavorite);
                cat.IsFavorite = newIsFavorite;
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error toggling favorite: {ex.Message}");
            }
        }

        [RelayCommand]
        async Task DeleteCat(Cat cat) {
            if (cat == null)
                return;

            try {
                bool confirm = await _dialogService.ShowConfirmationAsync(
                    "Delete Cat",
                    $"Are you sure you want to delete this cat (ID: {cat.Id})?",
                    "Delete",
                    "Cancel");

                if (!confirm)
                    return;

                await _catRepositoryService.DeleteCatAsync(cat.Id);
                Cats.Remove(cat);
            } catch (Exception ex) {
                await _dialogService.ShowAlertAsync("Error", $"Failed to delete cat: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        async Task NavigateToCatDetails(Cat cat) {
            if (cat == null)
                return;

            await _navigationService.GoToAsync($"{AppConstants.Routes.CatDetailsPage}?{AppConstants.QueryParameters.CatId}={cat.Id}");
        }

        [RelayCommand]
        void ToggleDisplayMode() {
            IsListMode = !IsListMode;
        }

        partial void OnSelectedBreedChanged(Breed? value) {
            Task.Run(async () => await LoadCatsAsync());
        }
    }
}
