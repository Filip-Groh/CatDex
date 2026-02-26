using CatDex.Services.Interfaces;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CatDex.Models.DTOs;

namespace CatDex.ViewModels {
    public partial class CatViewModel : ObservableObject {
        private readonly ICatRepository _repository;
        private int _currentPage = 0;

        public ObservableCollection<CatDTO> Cats { get; } = new();

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        public CatViewModel(ICatRepository repository) {
            _repository = repository;

            Task.Run(async () => await LoadMoreCats());
        }

        [RelayCommand]
        async Task LoadMoreCats() {
            if (IsBusy)
                return;

            try {
                IsBusy = true;

                var newCats = await _repository.GetCatsAsync(page: _currentPage, limit: 10);

                if (newCats.Count > 0) {
                    foreach (var cat in newCats)
                        Cats.Add(cat);

                    _currentPage++;
                }
            } finally {
                IsBusy = false;
            }
        }
    }
}
