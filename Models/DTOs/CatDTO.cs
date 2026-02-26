namespace CatDex.Models.DTOs {
    public class CatDTO {
        public required string Id { get; set; }
        public required string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
