using CatDex.Models.DTOs;
using CatDex.ViewModels;
using System.Globalization;

namespace CatDex.Converters {
    public class CatFavoriteStatusConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values.Length == 2 && values[0] is CatDTO cat && values[1] is DiscoveryViewModel viewModel) {
                if (viewModel.IsCatStored(cat.Id)) {
                    return viewModel.GetCatFavoriteStatus(cat.Id) ? "❤" : "♡";
                }
            }
            return "♡";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
