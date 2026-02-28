namespace CatDex.Models.DTOs
{
    public class CustomCatDTO
    {
        public required string Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ICollection<string>? BreedIds { get; set; }
        public required byte[] Bytes { get; set; }
    }
}
