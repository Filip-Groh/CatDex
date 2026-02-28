using System.Globalization;

namespace CatDex.Converters {
    public class CompareConverter : IValueConverter {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is int count && parameter is string paramStr && int.TryParse(paramStr, out int threshold)) {
                return count >= threshold;
            }
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
