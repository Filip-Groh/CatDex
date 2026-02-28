using System.Text.Json.Serialization;

namespace CatDex.Models.DTOs {
    public class BreedDTO {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Temperament { get; set; } = string.Empty;

        public string Origin { get; set; } = string.Empty;

        [JsonPropertyName("life_span")]
        public string LifeSpan { get; set; } = string.Empty;

        public WeightDTO? Weight { get; set; }

        [JsonPropertyName("alt_names")]
        public string AltNames { get; set; } = string.Empty;

        [JsonPropertyName("country_codes")]
        public string CountryCodes { get; set; } = string.Empty;

        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; } = string.Empty;

        [JsonPropertyName("wikipedia_url")]
        public string WikipediaUrl { get; set; } = string.Empty;

        public int Indoor { get; set; }

        public int Adaptability { get; set; }

        [JsonPropertyName("affection_level")]
        public int AffectionLevel { get; set; }

        [JsonPropertyName("child_friendly")]
        public int ChildFriendly { get; set; }

        [JsonPropertyName("dog_friendly")]
        public int DogFriendly { get; set; }

        [JsonPropertyName("energy_level")]
        public int EnergyLevel { get; set; }

        public int Grooming { get; set; }

        [JsonPropertyName("health_issues")]
        public int HealthIssues { get; set; }

        public int Intelligence { get; set; }

        [JsonPropertyName("shedding_level")]
        public int SheddingLevel { get; set; }

        [JsonPropertyName("social_needs")]
        public int SocialNeeds { get; set; }

        [JsonPropertyName("stranger_friendly")]
        public int StrangerFriendly { get; set; }

        public int Vocalisation { get; set; }

        public int Experimental { get; set; }

        public int Hairless { get; set; }

        public int Natural { get; set; }

        public int Rare { get; set; }

        public int Rex { get; set; }

        [JsonPropertyName("suppressed_tail")]
        public int SuppressedTail { get; set; }

        [JsonPropertyName("short_legs")]
        public int ShortLegs { get; set; }

        public int Hypoallergenic { get; set; }

        [JsonPropertyName("breed_group")]
        public string? BreedGroup { get; set; }

        [JsonPropertyName("cfa_url")]
        public string? CfaUrl { get; set; }

        [JsonPropertyName("vetstreet_url")]
        public string? VetstreetUrl { get; set; }

        [JsonPropertyName("vcahospitals_url")]
        public string? VcahospitalsUrl { get; set; }

        public int? Lap { get; set; }

        [JsonPropertyName("cat_friendly")]
        public int? CatFriendly { get; set; }

        public int? Bidability { get; set; }

        [JsonPropertyName("reference_image_id")]
        public string? ReferenceImageId { get; set; }
    }

    public class WeightDTO {
        public string Imperial { get; set; } = string.Empty;
        public string Metric { get; set; } = string.Empty;
    }
}
