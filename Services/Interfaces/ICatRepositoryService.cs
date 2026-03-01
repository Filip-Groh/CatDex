using CatDex.Models;
using CatDex.Models.DTOs;

namespace CatDex.Services.Interfaces {
    public interface ICatRepositoryService {
        public Task<Breed> GetBreedAsync(string id);
        public Task<ICollection<Breed>> GetBreedsAsync();

        public Task<ICollection<CatDTO>> GetNewCatsAsync(int page = 0, int limit = 10);
        public Task<DetailedCatDTO> GetDetailedCatAsync(string id);
        public Task<Cat?> GetCatByIdAsync(string id);
        public Task<ICollection<Cat>> GetStoredCatsAsync(string? breedId = null);
        public Task<ICollection<Cat>> GetFavoriteCatsAsync(string? breedId = null);
        public Task<Cat> StoreCatAsync(string id);
        public Task<Cat> CreateCatAsync(CustomCatDTO cat);
        public Task<Cat> DeleteCatAsync(string id);
        public Task<Cat> SetCatIsFavorite(string id, bool isFavorite);
        public Task<int> DeleteNonCreatedNonFavoriteCatsAsync();
        public Task<Cat> StoreCatImageAsync(string catId, byte[] imageBytes);
        public Task DeleteNonFavoriteCachedImagesAsync();
        public Task<(int total, int current)> CacheAllImagesAsync(IProgress<(int total, int current)> progress, CancellationToken cancellationToken);
    }
}
