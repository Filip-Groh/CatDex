using System.Globalization;

namespace CatDex.Converters {
    public class FavoriteToIconConverter : IValueConverter {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is bool isFavorite) {
                return isFavorite ? "❤" : "♡";
            }
            return "♡";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
