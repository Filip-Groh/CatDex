using CatDex.Constants;
using CatDex.Models.DTOs;
using CatDex.Services.Interfaces;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace CatDex.Services {
    public class ApiService : IApiService {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<BreedDTO> GetBreedAsync(string id) {
            return await ExecuteWithRetryAsync(async () => {
                var response = await _httpClient.GetFromJsonAsync<BreedDTO>($"{AppConstants.Api.Endpoints.Breeds}/{id}");
                return response ?? throw new Exception("Failed to fetch cat data.");
            });
        }

        public async Task<ICollection<BreedDTO>> GetBreedsAsync() {
            return await ExecuteWithRetryAsync(async () => {
                var response = await _httpClient.GetFromJsonAsync<ICollection<BreedDTO>>(AppConstants.Api.Endpoints.Breeds);
                return response ?? throw new Exception("Failed to fetch cat data.");
            });
        }

        public async Task<DetailedCatDTO> GetCatAsync(string id) {
            return await ExecuteWithRetryAsync(async () => {
                var response = await _httpClient.GetFromJsonAsync<DetailedCatDTO>($"{AppConstants.Api.Endpoints.Images}/{id}");
                return response ?? throw new Exception("Failed to fetch cat data.");
            });
        }

        public async Task<ICollection<CatDTO>> GetCatsAsync(int page = 0, int limit = 10) {
            return await ExecuteWithRetryAsync(async () => {
                var response = await _httpClient.GetFromJsonAsync<ICollection<DetailedCatDTO>>($"{AppConstants.Api.Endpoints.ImagesSearch}?limit={limit}&page={page}&order=RAND&has_breeds=1");
                if (response == null) throw new Exception("Failed to fetch cat data.");
                return response.Cast<CatDTO>().ToList();
            });
        }

        private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation) {
            for (int attempt = 0; attempt <= AppConstants.Api.MaxRetries; attempt++) {
                try {
                    return await operation();
                } catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests) {
                    if (attempt == AppConstants.Api.MaxRetries) {
                        Debug.WriteLine($"Rate limit exceeded after {AppConstants.Api.MaxRetries} retries.");
                        throw new Exception("API rate limit exceeded. Please try again later.", ex);
                    }

                    var delay = AppConstants.Api.BaseDelayMs * (int)Math.Pow(2, attempt);
                    Debug.WriteLine($"Rate limit hit, retrying in {delay}ms (attempt {attempt + 1}/{AppConstants.Api.MaxRetries})");
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
