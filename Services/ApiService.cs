using CatDex.Models.DTOs;
using CatDex.Services.Interfaces;
using System.Net.Http.Json;

namespace CatDex.Services {
    public class ApiService : IApiService {
        private readonly HttpClient _httpClient;
        public ApiService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<ICollection<CatDTO>> GetCatsAsync(int page = 0, int limit = 10, CatOrder order = CatOrder.Descending) {
            var response = await _httpClient.GetFromJsonAsync<ICollection<CatDTO>>($"images/search?limit={limit}&page={page}&order={(order == CatOrder.Ascending ? "ASC" : "DESC")}");
            return response ?? throw new Exception("Failed to fetch cat data.");
        }
    }
}
