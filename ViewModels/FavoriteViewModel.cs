using CatDex.Models;
using CatDex.Services.Interfaces;

namespace CatDex.ViewModels {
    public partial class FavoriteViewModel : CatListViewModelBase {
        public FavoriteViewModel(ICatRepositoryService catRepositoryService, INavigationService navigationService, IDialogService dialogService)
            : base(catRepositoryService, navigationService, dialogService) {
        }

        protected override async Task<IEnumerable<Cat>> GetCatsAsync(string? breedId) {
            return await _catRepositoryService.GetFavoriteCatsAsync(breedId);
        }
    }
}
