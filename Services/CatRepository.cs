using CatDex.Data;
using CatDex.Models;
using CatDex.Models.DTOs;
using CatDex.Services.Interfaces;

namespace CatDex.Services {
    public class CatRepository : ICatRepository {
        private readonly AppDbContext _db;
        private readonly IApiService _api;

        public CatRepository(AppDbContext db, IApiService api) {
            _db = db;
            _api = api;
        }

        public Task<ICollection<CatDTO>> GetCatsAsync(int page = 0, int limit = 10, CatOrder order = CatOrder.Descending) {
            return _api.GetCatsAsync(page, limit, order);
        }
    }
}
