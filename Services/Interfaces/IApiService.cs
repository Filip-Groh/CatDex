using CatDex.Models.DTOs;

namespace CatDex.Services.Interfaces {
    public interface IApiService {
        public Task<BreedDTO> GetBreedAsync(string id);
        public Task<ICollection<BreedDTO>> GetBreedsAsync();

        public Task<DetailedCatDTO> GetCatAsync(string id);
        public Task<ICollection<CatDTO>> GetCatsAsync(int page = 0, int limit = 10);
    }
}
