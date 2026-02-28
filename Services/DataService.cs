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
                Temperament = breed.Temperament,
                Origin = breed.Origin,
                LifeSpan = breed.LifeSpan,
                WeightImperial = breed.Weight?.Imperial ?? string.Empty,
                WeightMetric = breed.Weight?.Metric ?? string.Empty,
                AltNames = breed.AltNames,
                CountryCodes = breed.CountryCodes,
                CountryCode = breed.CountryCode,
                WikipediaUrl = breed.WikipediaUrl,
                Indoor = breed.Indoor,
                Adaptability = breed.Adaptability,
                AffectionLevel = breed.AffectionLevel,
                ChildFriendly = breed.ChildFriendly,
                DogFriendly = breed.DogFriendly,
                EnergyLevel = breed.EnergyLevel,
                Grooming = breed.Grooming,
                HealthIssues = breed.HealthIssues,
                Intelligence = breed.Intelligence,
                SheddingLevel = breed.SheddingLevel,
                SocialNeeds = breed.SocialNeeds,
                StrangerFriendly = breed.StrangerFriendly,
                Vocalisation = breed.Vocalisation,
                Experimental = breed.Experimental,
                Hairless = breed.Hairless,
                Natural = breed.Natural,
                Rare = breed.Rare,
                Rex = breed.Rex,
                SuppressedTail = breed.SuppressedTail,
                ShortLegs = breed.ShortLegs,
                Hypoallergenic = breed.Hypoallergenic,
                BreedGroup = breed.BreedGroup,
                CfaUrl = breed.CfaUrl,
                VetstreetUrl = breed.VetstreetUrl,
                VcahospitalsUrl = breed.VcahospitalsUrl,
                Lap = breed.Lap,
                CatFriendly = breed.CatFriendly,
                Bidability = breed.Bidability,
                ReferenceImageId = breed.ReferenceImageId,
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
            existingBreed.Temperament = breed.Temperament;
            existingBreed.Origin = breed.Origin;
            existingBreed.LifeSpan = breed.LifeSpan;
            existingBreed.WeightImperial = breed.Weight?.Imperial ?? string.Empty;
            existingBreed.WeightMetric = breed.Weight?.Metric ?? string.Empty;
            existingBreed.AltNames = breed.AltNames;
            existingBreed.CountryCodes = breed.CountryCodes;
            existingBreed.CountryCode = breed.CountryCode;
            existingBreed.WikipediaUrl = breed.WikipediaUrl;
            existingBreed.Indoor = breed.Indoor;
            existingBreed.Adaptability = breed.Adaptability;
            existingBreed.AffectionLevel = breed.AffectionLevel;
            existingBreed.ChildFriendly = breed.ChildFriendly;
            existingBreed.DogFriendly = breed.DogFriendly;
            existingBreed.EnergyLevel = breed.EnergyLevel;
            existingBreed.Grooming = breed.Grooming;
            existingBreed.HealthIssues = breed.HealthIssues;
            existingBreed.Intelligence = breed.Intelligence;
            existingBreed.SheddingLevel = breed.SheddingLevel;
            existingBreed.SocialNeeds = breed.SocialNeeds;
            existingBreed.StrangerFriendly = breed.StrangerFriendly;
            existingBreed.Vocalisation = breed.Vocalisation;
            existingBreed.Experimental = breed.Experimental;
            existingBreed.Hairless = breed.Hairless;
            existingBreed.Natural = breed.Natural;
            existingBreed.Rare = breed.Rare;
            existingBreed.Rex = breed.Rex;
            existingBreed.SuppressedTail = breed.SuppressedTail;
            existingBreed.ShortLegs = breed.ShortLegs;
            existingBreed.Hypoallergenic = breed.Hypoallergenic;
            existingBreed.BreedGroup = breed.BreedGroup;
            existingBreed.CfaUrl = breed.CfaUrl;
            existingBreed.VetstreetUrl = breed.VetstreetUrl;
            existingBreed.VcahospitalsUrl = breed.VcahospitalsUrl;
            existingBreed.Lap = breed.Lap;
            existingBreed.CatFriendly = breed.CatFriendly;
            existingBreed.Bidability = breed.Bidability;
            existingBreed.ReferenceImageId = breed.ReferenceImageId;
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
