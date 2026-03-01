using CatDex.Models;
using CatDex.Models.DTOs;

namespace CatDex.Services.Interfaces {
    public interface IDataService {
        public Task<Breed?> GetBreedAsync(string id);
        public Task<ICollection<Breed>> GetBreedsAsync();
        public Task<Breed> CreateBreedAsync(BreedDTO breed);
        public Task<Breed> UpdateBreedAsync(string id, BreedDTO breed);

        public Task<Cat?> GetCatAsync(string id);
        public Task<ICollection<Cat>> GetCatsAsync(string? breedId = null);
        public Task<ICollection<Cat>> GetFavoriteCatsAsync(string? breedId = null);
        public Task<Cat> StoreCatAsync(DetailedCatDTO cat);
        public Task<Cat> CreateCatAsync(CustomCatDTO cat);
        public Task<Cat> UpdateCatAsync(string id, DetailedCatDTO cat);
        public Task<Cat> DeleteCatAsync(string id);
        public Task<Cat> SetCatIsFavorite(string id, bool isFavorite);
        public Task<int> DeleteNonCreatedNonFavoriteCatsAsync();
        public Task<Cat> StoreCatImageAsync(string catId, byte[] imageBytes);
        public Task DeleteCatImageAsync(string catId);
        public Task<ICollection<Cat>> GetCatsWithoutImagesAsync();
    }
}
