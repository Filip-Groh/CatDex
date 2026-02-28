using CatDex.ViewModels;

namespace CatDex.Views;

public partial class DiscoverPage : ContentPage
{
	public DiscoverPage(DiscoveryViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    private async void OnCollectionViewScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        var collectionView = (CollectionView)sender;
        var viewModel = (DiscoveryViewModel)BindingContext;
        
        int currentIndex = e.FirstVisibleItemIndex;
        
        if (currentIndex >= 0 && currentIndex < viewModel.Cats.Count)
        {
            var prevSelectedCat = viewModel.SelectedCat;
            viewModel.SelectedCat = viewModel.Cats[currentIndex];

            if (prevSelectedCat?.Id != viewModel.SelectedCat.Id) {
                await viewModel.OnCatSelected();
            }
        }
    }
}