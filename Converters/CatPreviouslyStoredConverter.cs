using CatDex.Models.DTOs;
using CatDex.ViewModels;
using System.Globalization;

namespace CatDex.Converters {
    public class CatPreviouslyStoredConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values.Length >= 2 && values[0] is CatDTO cat && values[1] is DiscoveryViewModel viewModel) {
                return viewModel.IsCatPreviouslyStored(cat.Id);
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
