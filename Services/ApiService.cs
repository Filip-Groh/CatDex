using CatDex.Models.DTOs;
using CatDex.Services.Interfaces;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace CatDex.Services {
    public class ApiService : IApiService {
        private readonly HttpClient _httpClient;
        private const int MaxRetries = 3;
        private const int BaseDelayMs = 1000;

        public ApiService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<BreedDTO> GetBreedAsync(string id) {
            return await ExecuteWithRetryAsync(async () => {
                var response = await _httpClient.GetFromJsonAsync<BreedDTO>($"breeds/{id}");
                return response ?? throw new Exception("Failed to fetch cat data.");
            });
        }

        public async Task<ICollection<BreedDTO>> GetBreedsAsync() {
            return await ExecuteWithRetryAsync(async () => {
                var response = await _httpClient.GetFromJsonAsync<ICollection<BreedDTO>>("breeds");
                return response ?? throw new Exception("Failed to fetch cat data.");
            });
        }

        public async Task<DetailedCatDTO> GetCatAsync(string id) {
            return await ExecuteWithRetryAsync(async () => {
                var response = await _httpClient.GetFromJsonAsync<DetailedCatDTO>($"images/{id}");
                return response ?? throw new Exception("Failed to fetch cat data.");
            });
        }

        public async Task<ICollection<CatDTO>> GetCatsAsync(int page = 0, int limit = 10) {
            return await ExecuteWithRetryAsync(async () => {
                var response = await _httpClient.GetFromJsonAsync<ICollection<DetailedCatDTO>>($"images/search?limit={limit}&page={page}&order=RAND&has_breeds=1");
                if (response == null) throw new Exception("Failed to fetch cat data.");
                return response.Cast<CatDTO>().ToList();
            });
        }

        private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation) {
            for (int attempt = 0; attempt <= MaxRetries; attempt++) {
                try {
                    return await operation();
                } catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests) {
                    if (attempt == MaxRetries) {
                        Debug.WriteLine($"Rate limit exceeded after {MaxRetries} retries.");
                        throw new Exception("API rate limit exceeded. Please try again later.", ex);
                    }

                    var delay = BaseDelayMs * (int)Math.Pow(2, attempt);
                    Debug.WriteLine($"Rate limit hit, retrying in {delay}ms (attempt {attempt + 1}/{MaxRetries})");
                    await Task.Delay(delay);
                } catch (HttpRequestException ex) {
                    Debug.WriteLine($"HTTP request failed: {ex.Message}");
                    throw;
                } catch (Exception ex) {
                    Debug.WriteLine($"API call failed: {ex.Message}");
                    throw;
                }
            }

            throw new Exception("Unexpected error in retry logic.");
        }
    }
}
