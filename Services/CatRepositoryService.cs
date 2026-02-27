using CatDex.Models;
using CatDex.Models.DTOs;
using CatDex.Services.Interfaces;

namespace CatDex.Services {
    public class CatRepositoryService : ICatRepositoryService {
        private readonly IDataService _data;
        private readonly IApiService _api;

        public CatRepositoryService(IDataService data, IApiService api) {
            _data = data;
            _api = api;
        }

        public async Task<Breed> GetBreedAsync(string id) {
            var breed = await _data.GetBreedAsync(id);

            if (breed != null && breed.InvalidationDate >= DateTime.Now) {
                return breed;
            }

            var fetchedBreed = await _api.GetBreedAsync(id) ?? throw new Exception($"Breed with id {id} not found in API.");

            if (breed?.InvalidationDate < DateTime.Now) {
                return await _data.UpdateBreedAsync(id, fetchedBreed);
            } else {
                return await _data.CreateBreedAsync(fetchedBreed);
            }
        }

        public async Task<ICollection<Breed>> GetBreedsAsync() {
            var breeds = await _data.GetBreedsAsync();

            if (breeds == null || breeds.Count == 0) {
                var fetchedBreeds = await _api.GetBreedsAsync();

                var createdBreeds = new List<Breed>();
                foreach (var breed in fetchedBreeds) {
                    createdBreeds.Add(await _data.CreateBreedAsync(breed));
                }

                return createdBreeds;
            }

            var updatedBreeds = await Task.WhenAll(breeds.Select(async breed => {
                if (breed.InvalidationDate < DateTime.Now) {
                    var fetchedBreed = await _api.GetBreedAsync(breed.Id);
                    if (fetchedBreed != null) {
                        return await _data.UpdateBreedAsync(breed.Id, fetchedBreed);
                    }
                }

                return breed;
            }));

            return updatedBreeds;
        }

        public Task<ICollection<CatDTO>> GetNewCatsAsync(int page = 0, int limit = 10) {
            return _api.GetCatsAsync(page, limit);
        }

        public async Task<ICollection<Cat>> GetStoredCatsAsync(string? breedId = null) {
            await GetBreedsAsync(); // Ensure breeds are up to date before fetching cats

            var cats = await _data.GetCatsAsync(breedId);

            var updatedCats = await Task.WhenAll(cats.Select(async cat => {
                if (cat.InvalidationDate < DateTime.Now) {
                    var fetchedCat = await _api.GetCatAsync(cat.Id);
                    if (fetchedCat != null) {
                        return await _data.UpdateCatAsync(cat.Id, fetchedCat);
                    }
                }

                return cat;
            }));

            return updatedCats;
        }

        public async Task<ICollection<Cat>> GetFavoriteCatsAsync(string? breedId = null) {
            await GetBreedsAsync(); // Ensure breeds are up to date before fetching cats

            var cats = await _data.GetFavoriteCatsAsync(breedId);

            var updatedCats = await Task.WhenAll(cats.Select(async cat => {
                if (cat.InvalidationDate < DateTime.Now) {
                    var fetchedCat = await _api.GetCatAsync(cat.Id);
                    if (fetchedCat != null) {
                        return await _data.UpdateCatAsync(cat.Id, fetchedCat);
                    }
                }

                return cat;
            }));

            return updatedCats;
        }

        public async Task<Cat> StoreCatAsync(string id) {
            var cat = await _data.GetCatAsync(id);
            
            if (cat != null && cat.InvalidationDate >= DateTime.Now) {
                return cat;
            }
            
            var fetchedCat = await _api.GetCatAsync(id) ?? throw new Exception($"Cat with id {id} not found in API.");
            
            if (cat?.InvalidationDate < DateTime.Now) {
                return await _data.UpdateCatAsync(id, fetchedCat);
            } else {
                return await _data.CreateCatAsync(fetchedCat);
            }
        }

        public async Task<Cat> DeleteCatAsync(string id) {           
            return await _data.DeleteCatAsync(id);
        }

        public async Task<Cat> SetCatIsFavorite(string id, bool isFavorite) {
            return await _data.SetCatIsFavorite(id, isFavorite);
        }
    }
}
