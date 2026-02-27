using System.ComponentModel.DataAnnotations;

namespace CatDex.Models {
    public class Breed {
        [Key]
        public required string Id { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public DateTime? InvalidationDate { get; set; }

        public ICollection<Cat> Cats { get; set; } = new List<Cat>();
    }
}
