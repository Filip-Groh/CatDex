using System.ComponentModel.DataAnnotations;

namespace CatDex.Models {
    public class Cat {
        [Key]
        public required string Id { get; set; }

        public bool IsFavorite { get; set; }

        public string? Url { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public ImageData? StoredImage { get; set; }

        public ICollection<Breed> Breeds { get; set; } = new List<Breed>();
    }
}
