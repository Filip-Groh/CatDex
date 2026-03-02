using CatDex.Constants;

namespace CatDex {
    public partial class AppShell : Shell {
        public AppShell() {
            InitializeComponent();

            Routing.RegisterRoute(AppConstants.Routes.DiscoverPage, typeof(Views.DiscoverPage));
            Routing.RegisterRoute(AppConstants.Routes.SeenPage, typeof(Views.SeenPage));
            Routing.RegisterRoute(AppConstants.Routes.CreatePage, typeof(Views.CreatePage));
            Routing.RegisterRoute(AppConstants.Routes.FavoritePage, typeof(Views.FavoritePage));
            Routing.RegisterRoute(AppConstants.Routes.SettingsPage, typeof(Views.SettingsPage));
            Routing.RegisterRoute(AppConstants.Routes.CatDetailsPage, typeof(Views.CatDetailsPage));
            Routing.RegisterRoute(AppConstants.Routes.FullScreenImagePage, typeof(Views.FullScreenImagePage));
        }
    }
}
