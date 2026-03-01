using CatDex.Models;
using CatDex.Models.DTOs;
using System.Globalization;

namespace CatDex.Converters {
    public class AdditionalBreedsConverter : IValueConverter {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is ICollection<Breed> breeds && breeds.Count > 1) {
                var additionalBreeds = breeds.Skip(1).Select(b => b.Name);
                return string.Join(", ", additionalBreeds);
            }
            if (value is ICollection<BreedDTO> breedDTOs && breedDTOs.Count > 1) {
                var additionalBreeds = breedDTOs.Skip(1).Select(b => b.Name);
                return string.Join(", ", additionalBreeds);
            }
            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
