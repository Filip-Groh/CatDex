using System.Globalization;

namespace CatDex.Converters {
    public class FavoriteToIconConverter : IValueConverter {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is bool isFavorite) {
                return isFavorite ? "favorite_full.svg" : "favorite_empty.svg";
            }
            return "favorite_empty.svg";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
