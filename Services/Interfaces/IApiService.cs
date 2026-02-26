using CatDex.Models.DTOs;

namespace CatDex.Services.Interfaces {
    public enum CatOrder {
        Ascending,
        Descending
    }

    public interface IApiService {
        public Task<ICollection<CatDTO>> GetCatsAsync(int page = 0, int limit = 10, CatOrder order = CatOrder.Descending);
    }
}
