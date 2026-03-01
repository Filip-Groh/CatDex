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
        private readonly IConnectivityService _connectivity;
        private int _currentPage = 0;

        public ObservableCollection<DetailedCatDTO> Cats { get; } = new();
        public Dictionary<string, bool> StoredCatsFavoriteStatus { get; } = new();
        public HashSet<string> PreviouslyStoredCats { get; } = new();

        [ObservableProperty]
        public partial int StorageStatusVersion { get; set; }

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial DetailedCatDTO? SelectedCat { get; set; }

        [ObservableProperty]
        public partial bool IsOffline { get; set; }

        public DiscoveryViewModel(ICatRepositoryService repository, IFileSaverService fileSaverService, IConnectivityService connectivity) {
            _repository = repository;
            _fileSaverService = fileSaverService;
            _connectivity = connectivity;

            IsOffline = !_connectivity.IsConnected;
            _connectivity.ConnectivityChanged += OnConnectivityChanged;

            Task.Run(async () => await OnThresholdReached());
        }

        private void OnConnectivityChanged(object? sender, bool isConnected) {
            IsOffline = !isConnected;

            // When coming back online, load cats if the collection is empty
            if (isConnected && Cats.Count == 0) {
                Task.Run(async () => await OnThresholdReached());
            }
        }

        [RelayCommand]
        async Task OnThresholdReached() {
            if (IsBusy || IsOffline)
                return;

            try {
                IsBusy = true;

                var newCats = await _repository.GetNewCatsAsync(page: _currentPage, limit: 10);

                if (newCats.Count > 0) {
                    var storedCats = await _repository.GetStoredCatsAsync();

                    foreach (var storedCat in storedCats) {
                        StoredCatsFavoriteStatus[storedCat.Id] = storedCat.IsFavorite;
                        PreviouslyStoredCats.Add(storedCat.Id);
                    }

                    // Convert basic CatDTO to DetailedCatDTO if needed
                    foreach (var cat in newCats) {
                        DetailedCatDTO detailedCat;

                        if (cat is DetailedCatDTO detailed) {
                            // Already detailed from search API
                            detailedCat = detailed;
                            Debug.WriteLine($"Cat {detailedCat.Id} from search: Breeds count = {detailedCat.Breeds?.Count ?? 0}");
                            if (detailedCat.Breeds != null && detailedCat.Breeds.Count > 0) {
                                Debug.WriteLine($"  First breed: {detailedCat.Breeds.First().Name}");
                            } else {
                                Debug.WriteLine($"  No breeds found, fetching detailed info...");
                                try {
                                    detailedCat = await _repository.GetDetailedCatAsync(cat.Id);
                                    Debug.WriteLine($"  After detailed fetch: Breeds count = {detailedCat.Breeds?.Count ?? 0}");
                                } catch (Exception ex) {
                                    Debug.WriteLine($"  Failed to fetch: {ex.Message}");
                                }
                            }
                        } else {
                            // Need to fetch detailed info
                            Debug.WriteLine($"Cat {cat.Id}: Basic CatDTO, fetching detailed...");
                            try {
                                detailedCat = await _repository.GetDetailedCatAsync(cat.Id);
                                Debug.WriteLine($"  Breeds count = {detailedCat.Breeds?.Count ?? 0}");
                            } catch (Exception ex) {
                                Debug.WriteLine($"  Failed to fetch detailed cat {cat.Id}: {ex.Message}");
                                // Fallback: create DetailedCatDTO without breeds
                                detailedCat = new DetailedCatDTO {
                                    Id = cat.Id,
                                    Url = cat.Url,
                                    Width = cat.Width,
                                    Height = cat.Height,
                                    Breeds = new List<BreedDTO>()
                                };
                            }
                        }

                        Cats.Add(detailedCat);
                    }

                    _currentPage++;
                }
            } finally {
                IsBusy = false;
            }
        }

        public async Task OnCatSelected() {
            Debug.WriteLine(SelectedCat?.Url);
            if (SelectedCat != null && !IsOffline) {
                try {
                    bool wasAlreadyStored = IsCatStored(SelectedCat.Id);
                    var storedCat = await _repository.StoreCatAsync(SelectedCat.Id);

                    if (!wasAlreadyStored) {
                        StoredCatsFavoriteStatus[storedCat.Id] = storedCat.IsFavorite;
                        StorageStatusVersion++;
                    }
                } catch (Exception ex) {
                    Debug.WriteLine($"Failed to store cat: {ex.Message}");
                }
            }
        }

        public bool IsCatStored(string catId) {
            return StoredCatsFavoriteStatus.ContainsKey(catId);
        }

        public bool GetCatFavoriteStatus(string catId) {
            return StoredCatsFavoriteStatus.TryGetValue(catId, out var isFavorite) && isFavorite;
        }

        public bool IsCatPreviouslyStored(string catId) {
            return PreviouslyStoredCats.Contains(catId);
        }

        [RelayCommand]
        async Task ToggleFavorite(DetailedCatDTO cat) {
            if (cat == null || !IsCatStored(cat.Id))
                return;

            try {
                var currentStatus = GetCatFavoriteStatus(cat.Id);
                var newIsFavorite = !currentStatus;
                await _repository.SetCatIsFavorite(cat.Id, newIsFavorite);

                StoredCatsFavoriteStatus[cat.Id] = newIsFavorite;
                StorageStatusVersion++;
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
