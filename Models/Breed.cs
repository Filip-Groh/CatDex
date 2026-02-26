using System.ComponentModel.DataAnnotations;

namespace CatDex.Models {
    public class Breed {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }

        public ICollection<Cat> Cats { get; set; } = new List<Cat>();
    }
}
