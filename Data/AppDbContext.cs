using CatDex.Models;
using Microsoft.EntityFrameworkCore;

namespace CatDex.Data {
    public class AppDbContext : DbContext {
        public DbSet<Cat> Cats { get; set; }
        public DbSet<Breed> Breeds { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
            SQLitePCL.Batteries_V2.Init();

        #if DEBUG
            Database.EnsureDeleted();
        #endif

            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Cat>()
                .HasOne(c => c.StoredImage)
                .WithOne(i => i.Cat)
                .HasForeignKey<ImageData>(i => i.Id);

            modelBuilder.Entity<Cat>()
                .HasMany(c => c.Breeds)
                .WithMany(b => b.Cats)
                .UsingEntity(j => j.ToTable("CatToBreed"));

            modelBuilder.Entity<ImageData>()
                .ToTable("ImageData");
        }
    }
}
