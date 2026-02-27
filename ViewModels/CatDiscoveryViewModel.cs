using CatDex.Services.Interfaces;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CatDex.Models.DTOs;
using System.Diagnostics;

namespace CatDex.ViewModels {
    public partial class CatDiscoveryViewModel : ObservableObject {
        private readonly ICatRepositoryService _repository;
        private int _currentPage = 0;

        public ObservableCollection<CatDTO> Cats { get; } = new();

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial CatDTO? SelectedCat { get; set; }

        public CatDiscoveryViewModel(ICatRepositoryService repository) {
            _repository = repository;

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
                await _repository.StoreCatAsync(SelectedCat.Id);
            }
        }
    }
}
