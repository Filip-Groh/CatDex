using CatDex.Models;
using CatDex.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CatDex.ViewModels {
    public partial class SearchViewModel : ObservableObject {
        private readonly ICatRepositoryService _catRepositoryService;

        public ObservableCollection<Cat> Cats { get; } = new();

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial bool IsRefreshing { get; set; }

        public SearchViewModel(ICatRepositoryService catRepositoryService) {
            _catRepositoryService = catRepositoryService;

            Task.Run(async () => await LoadCatsAsync());
        }

        async Task LoadCatsAsync() {
            if (IsBusy)
                return;

            try {
                IsBusy = true;

                var cats = await _catRepositoryService.GetStoredCatsAsync();

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
    }
}
