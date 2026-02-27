namespace CatDex.Models.DTOs {
    public class DetailedCatDTO : CatDTO {
        public ICollection<BreedDTO>? Breeds { get; set; }
    }
}
