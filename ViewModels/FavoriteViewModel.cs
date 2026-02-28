using CatDex.Models;
using CatDex.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CatDex.ViewModels {
    public partial class FavoriteViewModel : ObservableObject {
        private readonly ICatRepositoryService _catRepositoryService;

        public ObservableCollection<Cat> Cats { get; } = new();

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial bool IsRefreshing { get; set; }

        public FavoriteViewModel(ICatRepositoryService catRepositoryService) {
            _catRepositoryService = catRepositoryService;

            Task.Run(async () => await LoadCatsAsync());
        }

        async Task LoadCatsAsync() {
            if (IsBusy)
                return;

            try {
                IsBusy = true;

                var cats = await _catRepositoryService.GetFavoriteCatsAsync();

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
            IsRefreshing = true;
            try {
                await LoadCatsAsync();
            } finally {
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
        async Task NavigateToCatDetails(Cat cat) {
            if (cat == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(Views.CatDetailsPage)}?CatId={cat.Id}");
        }
    }
}
