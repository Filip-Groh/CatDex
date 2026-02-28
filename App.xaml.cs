using CatDex.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CatDex {
    public partial class App : Application {
        public App() {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState) {
            var themeService = Handler?.MauiContext?.Services?.GetService<IThemeService>();
            if (themeService != null) {
                var savedTheme = themeService.GetSavedTheme();
                UserAppTheme = savedTheme;
            }

            return new Window(new AppShell());
        }
    }
}