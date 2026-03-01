using CatDex.Models;
using CatDex.Models.DTOs;
using System.Globalization;

namespace CatDex.Converters {
    public class FirstBreedNameConverter : IValueConverter {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is ICollection<Breed> breeds && breeds.Count > 0) {
                return breeds.First().Name;
            }
            if (value is ICollection<BreedDTO> breedDTOs && breedDTOs.Count > 0) {
                return breedDTOs.First().Name;
            }
            return "Unknown";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
