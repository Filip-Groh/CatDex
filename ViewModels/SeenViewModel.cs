using CatDex.Models;
using CatDex.Services.Interfaces;

namespace CatDex.ViewModels {
    public partial class SeenViewModel : CatListViewModelBase {
        public SeenViewModel(ICatRepositoryService catRepositoryService, INavigationService navigationService, IDialogService dialogService)
            : base(catRepositoryService, navigationService, dialogService) {
        }

        protected override async Task<IEnumerable<Cat>> GetCatsAsync(string? breedId, int skip, int take) {
            return await _catRepositoryService.GetStoredCatsAsync(breedId, skip, take);
        }
    }
}
