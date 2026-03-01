using CatDex.Models;
using CatDex.Models.DTOs;
using CatDex.Services.Interfaces;
using System.Diagnostics;

namespace CatDex.Services {
    public class CatRepositoryService : ICatRepositoryService {
        private readonly IDataService _data;
        private readonly IApiService _api;
        private readonly IConnectivityService _connectivity;
        private const string StoreImagesKey = "store_images_preference";

        public CatRepositoryService(IDataService data, IApiService api, IConnectivityService connectivity) {
            _data = data;
            _api = api;
            _connectivity = connectivity;
        }

        public async Task<Breed> GetBreedAsync(string id) {
            var breed = await _data.GetBreedAsync(id);

            if (breed != null && breed.InvalidationDate >= DateTime.Now) {
                return breed;
            }

            if (!_connectivity.IsConnected) {
                if (breed != null) {
                    return breed;
                }
                throw new Exception("No internet connection and breed not found in local cache.");
            }

            try {
                var fetchedBreed = await _api.GetBreedAsync(id) ?? throw new Exception($"Breed with id {id} not found in API.");

                if (breed?.InvalidationDate < DateTime.Now) {
                    return await _data.UpdateBreedAsync(id, fetchedBreed);
                } else {
                    return await _data.CreateBreedAsync(fetchedBreed);
                }
            } catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase)) {
                Debug.WriteLine($"Rate limit hit when fetching breed {id}, using cached data if available.");
                if (breed != null) {
                    return breed;
                }
                throw new Exception("API rate limit exceeded. Please try again later.", ex);
            } catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException) {
                if (breed != null) {
                    return breed;
                }
                throw new Exception("Failed to fetch breed from API and no cached data available.", ex);
            }
        }

        public async Task<ICollection<Breed>> GetBreedsAsync() {
            var breeds = await _data.GetBreedsAsync();

            if (breeds == null || breeds.Count == 0) {
                if (!_connectivity.IsConnected) {
                    return Array.Empty<Breed>();
                }

                try {
                    var fetchedBreeds = await _api.GetBreedsAsync();

                    var createdBreeds = new List<Breed>();
                    foreach (var breed in fetchedBreeds) {
                        createdBreeds.Add(await _data.CreateBreedAsync(breed));
                    }

                    return createdBreeds;
                } catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase)) {
                    Debug.WriteLine("Rate limit hit when fetching breeds, returning empty collection.");
                    return Array.Empty<Breed>();
                } catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException) {
                    return Array.Empty<Breed>();
                }
            }

            if (!_connectivity.IsConnected) {
                return breeds;
            }

            var updatedBreeds = await Task.WhenAll(breeds.Select(async breed => {
                if (breed.InvalidationDate < DateTime.Now) {
                    try {
                        var fetchedBreed = await _api.GetBreedAsync(breed.Id);
                        if (fetchedBreed != null) {
                            return await _data.UpdateBreedAsync(breed.Id, fetchedBreed);
                        }
                    } catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase)) {
                        Debug.WriteLine($"Rate limit hit when updating breed {breed.Id}, using cached data.");
                        return breed;
                    } catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException) {
                        // Return cached breed if API call fails
                        return breed;
                    }
                }

                return breed;
            }));

            return updatedBreeds;
        }

        public async Task<ICollection<CatDTO>> GetNewCatsAsync(int page = 0, int limit = 10) {
            if (!_connectivity.IsConnected) {
                return Array.Empty<CatDTO>();
            }

            try {
                return await _api.GetCatsAsync(page, limit);
            } catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase)) {
                Debug.WriteLine($"Rate limit hit when fetching new cats (page {page}), returning empty collection.");
                return Array.Empty<CatDTO>();
            } catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException) {
                return Array.Empty<CatDTO>();
            }
        }

        public async Task<DetailedCatDTO> GetDetailedCatAsync(string id) {
            if (!_connectivity.IsConnected) {
                throw new Exception("No internet connection.");
            }

            try {
                return await _api.GetCatAsync(id);
            } catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase)) {
                Debug.WriteLine($"Rate limit hit when fetching detailed cat {id}.");
                throw new Exception("API rate limit exceeded. Please try again later.", ex);
            } catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException) {
                throw new Exception("Failed to fetch detailed cat data.", ex);
            }
        }

        public async Task<Cat?> GetCatByIdAsync(string id) {
            await GetBreedsAsync(); // Ensure breeds are up to date

            var cat = await _data.GetCatAsync(id);

            if (cat != null) {
                // Custom cats have no InvalidationDate (null), so they should always be returned as-is
                if (cat.InvalidationDate == null) {
                    return cat;
                }

                // For API cats, check if they're still valid
                if (cat.InvalidationDate >= DateTime.Now) {
                    return cat;
                }
            }

            if (!_connectivity.IsConnected) {
                return cat;
            }

            DetailedCatDTO? fetchedCat = null;
            try {
                fetchedCat = await _api.GetCatAsync(id);
            } catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase)) {
                Debug.WriteLine($"Rate limit hit when fetching cat {id}, using cached data if available.");
                return cat;
            } catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException) {
                return cat;
            }

            if (fetchedCat == null) {
                return cat;
            }

            Cat storedCat;
            if (cat?.InvalidationDate < DateTime.Now) {
                storedCat = await _data.UpdateCatAsync(id, fetchedCat);
            } else {
                storedCat = await _data.StoreCatAsync(fetchedCat);
            }

            var preference = Preferences.Get(StoreImagesKey, "favorites");
            if ((preference == "all" || (preference == "favorites" && storedCat.IsFavorite)) && 
                storedCat.StoredImage == null && !storedCat.IsUserCreated) {
                await DownloadAndStoreCatImageAsync(storedCat);
            }

            return storedCat;
        }

        public async Task<ICollection<Cat>> GetStoredCatsAsync(string? breedId = null) {
            await GetBreedsAsync(); // Ensure breeds are up to date before fetching cats

            var cats = await _data.GetCatsAsync(breedId);

            if (!_connectivity.IsConnected) {
                return cats;
            }

            var updatedCats = await Task.WhenAll(cats.Select(async cat => {
                if (cat.InvalidationDate < DateTime.Now) {
                    try {
                        var fetchedCat = await _api.GetCatAsync(cat.Id);
                        if (fetchedCat != null) {
                            return await _data.UpdateCatAsync(cat.Id, fetchedCat);
                        }
                    } catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase)) {
                        Debug.WriteLine($"Rate limit hit when updating cat {cat.Id}, using cached data.");
                        return cat;
                    } catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException) {
                        // Return cached cat if API call fails
                    }
                }

                return cat;
            }));

            return updatedCats;
        }

        public async Task<ICollection<Cat>> GetFavoriteCatsAsync(string? breedId = null) {
            await GetBreedsAsync(); // Ensure breeds are up to date before fetching cats

            var cats = await _data.GetFavoriteCatsAsync(breedId);

            if (!_connectivity.IsConnected) {
                return cats;
            }

            var updatedCats = await Task.WhenAll(cats.Select(async cat => {
                // Skip update for custom cats (InvalidationDate == null)
                if (cat.InvalidationDate != null && cat.InvalidationDate < DateTime.Now) {
                    try {
                        var fetchedCat = await _api.GetCatAsync(cat.Id);
                        if (fetchedCat != null) {
                            return await _data.UpdateCatAsync(cat.Id, fetchedCat);
                        }
                    } catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase)) {
                        Debug.WriteLine($"Rate limit hit when updating favorite cat {cat.Id}, using cached data.");
                        return cat;
                    } catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException) {
                        // Return cached cat if API call fails
                    }
                }

                return cat;
            }));

            return updatedCats;
        }

        public async Task<Cat> StoreCatAsync(string id) {
            var cat = await _data.GetCatAsync(id);

            if (cat != null) {
                // Custom cats have no InvalidationDate (null), so they should always be returned as-is
                if (cat.InvalidationDate == null) {
                    return cat;
                }

                // For API cats, check if they're still valid
                if (cat.InvalidationDate >= DateTime.Now) {
                    return cat;
                }
            }

            if (!_connectivity.IsConnected) {
                if (cat != null) {
                    return cat;
                }
                throw new Exception("No internet connection and cat not found in local cache.");
            }

            DetailedCatDTO? fetchedCat = null;
            try {
                fetchedCat = await _api.GetCatAsync(id);
            } catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase)) {
                Debug.WriteLine($"Rate limit hit when storing cat {id}.");
                if (cat != null) {
                    return cat;
                }
                throw new Exception("API rate limit exceeded. Please try again later.", ex);
            } catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException) {
                if (cat != null) {
                    return cat;
                }
                throw new Exception("Failed to fetch cat from API and no cached data available.", ex);
            }

            if (fetchedCat == null) {
                throw new Exception($"Cat with id {id} not found in API.");
            }

            Cat storedCat;
            if (cat?.InvalidationDate < DateTime.Now) {
                storedCat = await _data.UpdateCatAsync(id, fetchedCat);
            } else {
                storedCat = await _data.StoreCatAsync(fetchedCat);
            }

            var preference = Preferences.Get(StoreImagesKey, "favorites");
            if (preference == "all" && storedCat.StoredImage == null && !storedCat.IsUserCreated) {
                await DownloadAndStoreCatImageAsync(storedCat);
            }

            return storedCat;
        }

        public async Task<Cat> CreateCatAsync(CustomCatDTO cat) {
            return await _data.CreateCatAsync(cat);
        }

        public async Task<Cat> DeleteCatAsync(string id) {           
            return await _data.DeleteCatAsync(id);
        }

        public async Task<Cat> SetCatIsFavorite(string id, bool isFavorite) {
            var cat = await _data.SetCatIsFavorite(id, isFavorite);

            if (isFavorite) {
                // Always cache image when favoriting
                if (cat.StoredImage == null && !cat.IsUserCreated) {
                    await DownloadAndStoreCatImageAsync(cat);
                }
            } else {
                // Delete image when unfavoriting (if caching favorites only)
                var preference = Preferences.Get(StoreImagesKey, "favorites");
                if (preference == "favorites" && cat.StoredImage != null && !cat.IsUserCreated) {
                    await _data.DeleteCatImageAsync(cat.Id);
                }
            }

            return cat;
        }

        private async Task DownloadAndStoreCatImageAsync(Cat cat) {
            if (string.IsNullOrEmpty(cat.Url))
                return;

            if (!_connectivity.IsConnected)
                return;

            try {
                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(cat.Url);
                await _data.StoreCatImageAsync(cat.Id, imageBytes);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException) {
                System.Diagnostics.Debug.WriteLine($"Failed to download and store image for cat {cat.Id}: {ex.Message}");
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Unexpected error downloading image for cat {cat.Id}: {ex.Message}");
            }
        }

        public async Task DeleteNonFavoriteCachedImagesAsync() {
            var cats = await _data.GetCatsAsync();

            foreach (var cat in cats) {
                if (!cat.IsFavorite && !cat.IsUserCreated && cat.StoredImage != null) {
                    await _data.DeleteCatImageAsync(cat.Id);
                }
            }
        }

        public async Task<(int total, int current)> CacheAllImagesAsync(IProgress<(int total, int current)> progress, CancellationToken cancellationToken) {
            var catsWithoutImages = await _data.GetCatsWithoutImagesAsync();
            var total = catsWithoutImages.Count;
            var current = 0;

            foreach (var cat in catsWithoutImages) {
                if (cancellationToken.IsCancellationRequested) {
                    break;
                }

                if (!string.IsNullOrEmpty(cat.Url)) {
                    await DownloadAndStoreCatImageAsync(cat);
                }

                current++;
                progress?.Report((total, current));
            }

            return (total, current);
        }

        public async Task<int> DeleteNonCreatedNonFavoriteCatsAsync() {
            return await _data.DeleteNonCreatedNonFavoriteCatsAsync();
        }

        public async Task<Cat> StoreCatImageAsync(string catId, byte[] imageBytes) {
            return await _data.StoreCatImageAsync(catId, imageBytes);
        }
    }
}
