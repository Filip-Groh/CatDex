using CatDex.Models.DTOs;

namespace CatDex.Services.Interfaces {
    public interface ICatRepository {
        public Task<ICollection<CatDTO>> GetCatsAsync(int page = 0, int limit = 10, CatOrder order = CatOrder.Descending);
    }
}
