using CatDex.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CatDex.ViewModels {
    public partial class SettingsViewModel : ObservableObject {
        private readonly ICatRepositoryService _catRepositoryService;
        private readonly IThemeService _themeService;

        private const string StoreImagesKey = "store_images_preference";
        private CancellationTokenSource? _cachingCancellationTokenSource;

        [ObservableProperty]
        public partial int TotalCats { get; set; }

        [ObservableProperty]
        public partial int FavoriteCats { get; set; }

        [ObservableProperty]
        public partial int CreatedCats { get; set; }

        [ObservableProperty]
        public partial int TotalStoredImages { get; set; }

        [ObservableProperty]
        public partial int UserCreatedImages { get; set; }

        [ObservableProperty]
        public partial int CachedImages { get; set; }

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial bool IsRefreshing { get; set; }

        [ObservableProperty]
        public partial string SelectedTheme { get; set; }

        [ObservableProperty]
        public partial bool CacheAllImages { get; set; }

        [ObservableProperty]
        private bool _isCaching;

        [ObservableProperty]
        private int _cachingProgress;

        [ObservableProperty]
        private int _cachingTotal;

        public double CachingProgressRatio => CachingTotal > 0 ? (double)CachingProgress / CachingTotal : 0;

        public ObservableCollection<string> ThemeOptions { get; } = new ObservableCollection<string> 
        { 
            "System", 
            "Light", 
            "Dark" 
        };

        public SettingsViewModel(ICatRepositoryService catRepositoryService, IThemeService themeService) {
            _catRepositoryService = catRepositoryService;
            _themeService = themeService;

            SelectedTheme = _themeService.GetThemeName();
            LoadImageStoragePreference();

            Task.Run(async () => await LoadStatisticsAsync());
        }

        void LoadImageStoragePreference() {
            var preference = Preferences.Get(StoreImagesKey, "favorites");
            CacheAllImages = preference == "all";
        }

        async partial void OnCacheAllImagesChanged(bool value) {
            Preferences.Set(StoreImagesKey, value ? "all" : "favorites");

            if (value) {
                // Switching to "cache all" - start caching all images
                await StartCachingAllImagesAsync();
            } else {
                // Switching to "cache favorites only" - cancel caching and delete non-favorite images
                await CancelCachingAndCleanupAsync();
            }
        }

        partial void OnSelectedThemeChanged(string value) {
            if (string.IsNullOrEmpty(value))
                return;

            var theme = value switch {
                "Light" => AppTheme.Light,
                "Dark" => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };

            _themeService.SetTheme(theme);
        }

        async Task LoadStatisticsAsync() {
            if (IsBusy)
                return;

            try {
                IsBusy = true;

                var cats = await _catRepositoryService.GetStoredCatsAsync();

                TotalCats = cats.Count;
                FavoriteCats = cats.Count(cat => cat.IsFavorite);
                CreatedCats = cats.Count(cat => cat.IsUserCreated);

                // Image statistics
                var catsWithImages = cats.Where(cat => cat.StoredImage != null).ToList();
                TotalStoredImages = catsWithImages.Count;
                UserCreatedImages = catsWithImages.Count(cat => cat.IsUserCreated);
                CachedImages = catsWithImages.Count(cat => !cat.IsUserCreated);
            } finally {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task Refresh() {
            try {
                await LoadStatisticsAsync();
            } finally {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        async Task DeleteNonCreatedNonFavorite() {
            if (IsBusy)
                return;

            try {
                bool confirm = await Shell.Current.DisplayAlertAsync(
                    "Delete Cats",
                    "Are you sure you want to delete all non-created and non-favorite cats?",
                    "Delete",
                    "Cancel");

                if (!confirm)
                    return;

                IsBusy = true;

                var deletedCount = await _catRepositoryService.DeleteNonCreatedNonFavoriteCatsAsync();

                await Shell.Current.DisplayAlertAsync(
                    "Success",
                    $"Deleted {deletedCount} cat(s)",
                    "OK");

                await LoadStatisticsAsync();
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error deleting cats: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to delete cats: {ex.Message}", "OK");
            } finally {
                IsBusy = false;
            }
        }

        async Task StartCachingAllImagesAsync() {
            if (IsCaching)
                return;

            try {
                IsCaching = true;
                CachingProgress = 0;
                CachingTotal = 0;
                OnPropertyChanged(nameof(CachingProgressRatio));

                _cachingCancellationTokenSource = new CancellationTokenSource();

                var progress = new Progress<(int total, int current)>(p => {
                    CachingTotal = p.total;
                    CachingProgress = p.current;
                    OnPropertyChanged(nameof(CachingProgressRatio));
                });

                await _catRepositoryService.CacheAllImagesAsync(progress, _cachingCancellationTokenSource.Token);
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error caching images: {ex.Message}");
            } finally {
                IsCaching = false;
                CachingProgress = 0;
                CachingTotal = 0;
                OnPropertyChanged(nameof(CachingProgressRatio));
                _cachingCancellationTokenSource?.Dispose();
                _cachingCancellationTokenSource = null;
            }
        }

        async Task CancelCachingAndCleanupAsync() {
            // Cancel ongoing caching
            _cachingCancellationTokenSource?.Cancel();

            // Wait a bit for cancellation to complete
            if (IsCaching) {
                await Task.Delay(500);
            }

            try {
                // Delete images for non-favorite, non-user-created cats
                await _catRepositoryService.DeleteNonFavoriteCachedImagesAsync();
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error deleting cached images: {ex.Message}");
            }
        }
    }
}
