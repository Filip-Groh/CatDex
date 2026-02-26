using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatDex.Models {
    public class ImageData {
        [Key]
        public required string Id { get; set; }

        public required byte[] Bytes { get; set; }

        [ForeignKey(nameof(Id))]
        public required Cat Cat { get; set; }
    }
}
