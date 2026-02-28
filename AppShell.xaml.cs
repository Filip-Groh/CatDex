namespace CatDex {
    public partial class AppShell : Shell {
        public AppShell() {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Views.DiscoverPage), typeof(Views.DiscoverPage));
            Routing.RegisterRoute(nameof(Views.SearchPage), typeof(Views.SearchPage));
            Routing.RegisterRoute(nameof(Views.CreatePage), typeof(Views.CreatePage));
            Routing.RegisterRoute(nameof(Views.FavoritePage), typeof(Views.FavoritePage));
            Routing.RegisterRoute(nameof(Views.SettingsPage), typeof(Views.SettingsPage));
            Routing.RegisterRoute(nameof(Views.CatDetailsPage), typeof(Views.CatDetailsPage));
        }
    }
}
