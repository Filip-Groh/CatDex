using CatDex.Data;
using CatDex.Models;
using CatDex.Models.DTOs;
using CatDex.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatDex.Services {
    public class DataService : IDataService {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public DataService(IDbContextFactory<AppDbContext> contextFactory) {
            _contextFactory = contextFactory;
        }

        public async Task<Breed> GetBreedAsync(string id) {
            await using var db = await _contextFactory.CreateDbContextAsync();
            var breed = await db.Breeds.AsNoTracking().Where(breed => breed.Id == id).FirstOrDefaultAsync();
            return breed;
        }

        public async Task<ICollection<Breed>> GetBreedsAsync() {
            await using var db = await _contextFactory.CreateDbContextAsync();
            var breeds = await db.Breeds.AsNoTracking().ToArrayAsync();
            return breeds;
        }

        public async Task<Breed> CreateBreedAsync(BreedDTO breed) {
            await using var db = await _contextFactory.CreateDbContextAsync();
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

            db.Breeds.Add(createdBreed);

            await db.SaveChangesAsync();

            db.Entry(createdBreed).State = EntityState.Detached;

            return createdBreed;
        }

        public async Task<Breed> UpdateBreedAsync(string id, BreedDTO breed) {
            await using var db = await _contextFactory.CreateDbContextAsync();
            var existingBreed = await db.Breeds.FindAsync(id);

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

            await db.SaveChangesAsync();

            db.Entry(existingBreed).State = EntityState.Detached;

            return existingBreed;
        }

        public async Task<Cat?> GetCatAsync(string id) {
            try {
                await using var db = await _contextFactory.CreateDbContextAsync();
                var cat = await db.Cats.AsNoTracking()
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
            await using var db = await _contextFactory.CreateDbContextAsync();
            var cats = db.Cats.AsNoTracking()
                .Include(cat => cat.Breeds)
                .Include(cat => cat.StoredImage)
                .AsQueryable();

            if (!string.IsNullOrEmpty(breedId)) {
                cats = cats.Where(cat => cat.Breeds.Any(breed => breed.Id == breedId));
            }

            return await cats.ToArrayAsync();
        }

        public async Task<ICollection<Cat>> GetFavoriteCatsAsync(string? breedId = null) {
            await using var db = await _contextFactory.CreateDbContextAsync();
            var cats = db.Cats.AsNoTracking()
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
            await using var db = await _contextFactory.CreateDbContextAsync();
            var createdCat = new Cat {
                Id = cat.Id,
                Url = cat.Url,
                Width = cat.Width,
                Height = cat.Height,
                IsFavorite = false,
                IsUserCreated = false,
                InvalidationDate = DateTime.Now.AddDays(1),
            };

            if (cat.Breeds != null && cat.Breeds.Count > 0) {
                var breedIds = cat.Breeds.Select(breed => breed.Id);
                var breeds = await db.Breeds
                    .Where(b => breedIds.Contains(b.Id))
                    .ToArrayAsync();

                createdCat.Breeds = breeds;
            }

            db.Add(createdCat);

            await db.SaveChangesAsync();

            db.Entry(createdCat).State = EntityState.Detached;

            return createdCat;
        }

        public async Task<Cat> CreateCatAsync(CustomCatDTO cat) {
            await using var db = await _contextFactory.CreateDbContextAsync();
            var catObject = new Cat {
                Id = cat.Id,
                Url = null,
                Width = cat.Width,
                Height = cat.Height,
                IsFavorite = false,
                IsUserCreated = true,
                InvalidationDate = null,
            };

            var image = new ImageData {
                Id = cat.Id,
                Bytes = cat.Bytes,
                Cat = catObject
            };

            catObject.StoredImage = image;

            db.Images.Add(image);

            var breeds = await db.Breeds.Where(breed => cat.BreedIds != null && cat.BreedIds.Contains(breed.Id)).ToArrayAsync();

            catObject.Breeds = breeds;

            await db.SaveChangesAsync();

            db.Entry(catObject).State = EntityState.Detached;

            return catObject;
        }

        public async Task<Cat> UpdateCatAsync(string id, DetailedCatDTO cat) {
            await using var db = await _contextFactory.CreateDbContextAsync();
            var existingCat = await db.Cats.FindAsync(id);

            if (existingCat == null) {
                throw new InvalidOperationException($"Breed with ID {id} not found.");
            }

            existingCat.Url = cat.Url;
            existingCat.Width = cat.Width;
            existingCat.Height = cat.Height;
            existingCat.InvalidationDate = DateTime.Now.AddDays(1);

            await db.SaveChangesAsync();

            db.Entry(existingCat).State = EntityState.Detached;

            return existingCat;
        }

        public async Task<Cat> DeleteCatAsync(string id) {
            await using var db = await _contextFactory.CreateDbContextAsync();
            var existingCat = await db.Cats.Include(cat => cat.Breeds).FirstAsync(cat => cat.Id == id);
            
            if (existingCat == null) {
                throw new InvalidOperationException($"Cat with ID {id} not found.");
            }

            db.Cats.Remove(existingCat);

            await db.SaveChangesAsync();

            db.Entry(existingCat).State = EntityState.Detached;

            return existingCat;
        }

        public async Task<Cat> SetCatIsFavorite(string id, bool isFavorite) {
            await using var db = await _contextFactory.CreateDbContextAsync();
            var existingCat = await db.Cats.FindAsync(id);

            if (existingCat == null) {
                throw new InvalidOperationException($"Breed with ID {id} not found.");
            }

            existingCat.IsFavorite = isFavorite;

            await db.SaveChangesAsync();

            db.Entry(existingCat).State = EntityState.Detached;

            return existingCat;
        }

        public async Task<int> DeleteNonCreatedNonFavoriteCatsAsync() {
            await using var db = await _contextFactory.CreateDbContextAsync();
            var catsToDelete = await db.Cats
                .Include(cat => cat.StoredImage)
                .Where(cat => !cat.IsFavorite && !cat.IsUserCreated)
                .ToListAsync();

            var count = catsToDelete.Count;

            db.Cats.RemoveRange(catsToDelete);

            await db.SaveChangesAsync();

            return count;
        }

        public async Task<Cat> StoreCatImageAsync(string catId, byte[] imageBytes) {
            await using var db = await _contextFactory.CreateDbContextAsync();
            var existingCat = await db.Cats
                .Include(cat => cat.StoredImage)
                .FirstOrDefaultAsync(cat => cat.Id == catId);

            if (existingCat == null) {
                throw new InvalidOperationException($"Cat with ID {catId} not found.");
            }

            if (existingCat.StoredImage != null) {
                existingCat.StoredImage.Bytes = imageBytes;
            } else {
                var image = new ImageData {
                    Id = catId,
                    Bytes = imageBytes,
                    Cat = existingCat
                };

                existingCat.StoredImage = image;
                db.Images.Add(image);
            }

            await db.SaveChangesAsync();

            db.Entry(existingCat).State = EntityState.Detached;

            return existingCat;
        }

        public async Task DeleteCatImageAsync(string catId) {
            await using var db = await _contextFactory.CreateDbContextAsync();
            var image = await db.Images.FindAsync(catId);

            if (image != null) {
                db.Images.Remove(image);
                await db.SaveChangesAsync();
            }
        }

        public async Task<ICollection<Cat>> GetCatsWithoutImagesAsync() {
            await using var db = await _contextFactory.CreateDbContextAsync();
            var cats = await db.Cats
                .AsNoTracking()
                .Include(cat => cat.Breeds)
                .Include(cat => cat.StoredImage)
                .Where(cat => !cat.IsUserCreated && cat.StoredImage == null)
                .ToListAsync();

            return cats;
        }
    }
}
