using CatDex.Data;
using CatDex.Models;
using CatDex.Models.DTOs;
using CatDex.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatDex.Services {
    public class DataService : IDataService {
        private readonly AppDbContext _db;

        public DataService(AppDbContext db) {
            _db = db;
        }

        public async Task<Breed> GetBreedAsync(string id) {
            var breed = await _db.Breeds.AsNoTracking().Where(breed => breed.Id == id).FirstAsync();
            return breed;
        }

        public async Task<ICollection<Breed>> GetBreedsAsync() {
            var breeds = await _db.Breeds.AsNoTracking().ToArrayAsync();
            return breeds;
        }

        public async Task<Breed> CreateBreedAsync(BreedDTO breed) {
            var createdBreed = new Breed {
                Id = breed.Id,
                Name = breed.Name,
                Description = breed.Description,
                InvalidationDate = DateTime.Now.AddDays(1),
            };

            _db.Breeds.Add(createdBreed);

            await _db.SaveChangesAsync();

            _db.Entry(createdBreed).State = EntityState.Detached;

            return createdBreed;
        }

        public async Task<Breed> UpdateBreedAsync(string id, BreedDTO breed) {
            var existingBreed = await _db.Breeds.FindAsync(id);
            
            if (existingBreed == null) {
                throw new InvalidOperationException($"Breed with ID {id} not found.");
            }
            
            existingBreed.Name = breed.Name;
            existingBreed.Description = breed.Description;
            existingBreed.InvalidationDate = DateTime.Now.AddDays(1);
            
            await _db.SaveChangesAsync();

            _db.Entry(existingBreed).State = EntityState.Detached;

            return existingBreed;
        }

        public async Task<Cat?> GetCatAsync(string id) {
            try {
                var cat = await _db.Cats.AsNoTracking()
                    .Include(cat => cat.Breeds)
                    .Include(cat => cat.StoredImage)
                    .Where(cat => cat.Id == id)
                    .FirstAsync();
                return cat;
            } catch {
                return null;
            }
        }

        public async Task<ICollection<Cat>> GetCatsAsync(string? breedId = null) {
            var cats = _db.Cats.AsNoTracking()
                .Include(cat => cat.Breeds)
                .Include(cat => cat.StoredImage)
                .AsQueryable();

            if (!string.IsNullOrEmpty(breedId)) {
                cats = cats.Where(cat => cat.Breeds.Any(breed => breed.Id == breedId));
            }

            return await cats.ToArrayAsync();
        }

        public async Task<ICollection<Cat>> GetFavoriteCatsAsync(string? breedId = null) {
            var cats = _db.Cats.AsNoTracking()
                .Include(cat => cat.Breeds)
                .Include(cat => cat.StoredImage)
                .AsQueryable()
                .Where(cat => cat.IsFavorite);

            if (!string.IsNullOrEmpty(breedId)) {
                cats = cats.Where(cat => cat.Breeds.Any(breed => breed.Id == breedId));
            }

            return await cats.ToArrayAsync();
        }

        public async Task<Cat> StoreCatAsync(DetailedCatDTO cat) {
            var createdCat = new Cat {
                Id = cat.Id,
                Url = cat.Url,
                Width = cat.Width,
                Height = cat.Height,
                IsFavorite = false,
                InvalidationDate = DateTime.Now.AddDays(1),
            };

            if (cat.Breeds != null && cat.Breeds.Count > 0) {
                var breedIds = cat.Breeds.Select(breed => breed.Id);
                var breeds = await _db.Breeds
                    .Where(b => breedIds.Contains(b.Id))
                    .ToArrayAsync();
                
                createdCat.Breeds = breeds;
            }

            _db.Add(createdCat);

            await _db.SaveChangesAsync();

            _db.Entry(createdCat).State = EntityState.Detached;

            return createdCat;
        }

        public async Task<Cat> CreateCatAsync(CustomCatDTO cat) {
            var catObject = new Cat {
                Id = cat.Id,
                Url = null,
                Width = cat.Width,
                Height = cat.Height,
                IsFavorite = false,
                InvalidationDate = null,
            };

            var image = new ImageData {
                Id = cat.Id,
                Bytes = cat.Bytes,
                Cat = catObject
            };

            catObject.StoredImage = image;

            _db.Images.Add(image);

            var breeds = await _db.Breeds.Where(breed => cat.BreedIds != null && cat.BreedIds.Contains(breed.Id)).ToArrayAsync();

            catObject.Breeds = breeds;

            await _db.SaveChangesAsync();

            _db.Entry(catObject).State = EntityState.Detached;

            return catObject;
        }

        public async Task<Cat> UpdateCatAsync(string id, DetailedCatDTO cat) {
            var existingCat = await _db.Cats.FindAsync(id);

            if (existingCat == null) {
                throw new InvalidOperationException($"Breed with ID {id} not found.");
            }

            existingCat.Url = cat.Url;
            existingCat.Width = cat.Width;
            existingCat.Height = cat.Height;
            existingCat.InvalidationDate = DateTime.Now.AddDays(1);

            await _db.SaveChangesAsync();

            _db.Entry(existingCat).State = EntityState.Detached;

            return existingCat;
        }

        public async Task<Cat> DeleteCatAsync(string id) {
            var existingCat = await _db.Cats.Include(cat => cat.Breeds).FirstAsync(cat => cat.Id == id);
            
            if (existingCat == null) {
                throw new InvalidOperationException($"Cat with ID {id} not found.");
            }
            
            _db.Cats.Remove(existingCat);
            
            await _db.SaveChangesAsync();
            
            _db.Entry(existingCat).State = EntityState.Detached;
            
            return existingCat;
        }

        public async Task<Cat> SetCatIsFavorite(string id, bool isFavorite) {
            var existingCat = await _db.Cats.FindAsync(id);

            if (existingCat == null) {
                throw new InvalidOperationException($"Breed with ID {id} not found.");
            }

            existingCat.IsFavorite = isFavorite;

            await _db.SaveChangesAsync();

            _db.Entry(existingCat).State = EntityState.Detached;

            return existingCat;
        }

        public async Task<int> DeleteNonCreatedNonFavoriteCatsAsync() {
            var catsToDelete = await _db.Cats
                .Include(cat => cat.StoredImage)
                .Where(cat => !cat.IsFavorite && cat.StoredImage == null)
                .ToListAsync();

            var count = catsToDelete.Count;

            _db.Cats.RemoveRange(catsToDelete);

            await _db.SaveChangesAsync();

            return count;
        }
    }
}
