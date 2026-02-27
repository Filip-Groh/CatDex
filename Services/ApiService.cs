using CatDex.Models.DTOs;
using CatDex.Services.Interfaces;
using System.Net.Http.Json;

namespace CatDex.Services {
    public class ApiService : IApiService {
        private readonly HttpClient _httpClient;
        public ApiService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<BreedDTO> GetBreedAsync(string id) {
            var response = await _httpClient.GetFromJsonAsync<BreedDTO>($"breeds/{id}");
            return response ?? throw new Exception("Failed to fetch cat data.");
        }

        public async Task<ICollection<BreedDTO>> GetBreedsAsync() {
            var response = await _httpClient.GetFromJsonAsync<ICollection<BreedDTO>>("breeds");
            return response ?? throw new Exception("Failed to fetch cat data.");
        }

        public async Task<DetailedCatDTO> GetCatAsync(string id) {
            var response = await _httpClient.GetFromJsonAsync<DetailedCatDTO>($"images/{id}");
            return response ?? throw new Exception("Failed to fetch cat data.");
        }

        public async Task<ICollection<CatDTO>> GetCatsAsync(int page = 0, int limit = 10) {
            var response = await _httpClient.GetFromJsonAsync<ICollection<CatDTO>>($"images/search?limit={limit}&page={page}&order=RAND&has_breeds=1");
            return response ?? throw new Exception("Failed to fetch cat data.");
        }
    }
}
