using CatDex.Services.Interfaces;

namespace CatDex.Services {
    public class ThemeService : IThemeService {
        private const string ThemeKey = "app_theme";

        public AppTheme GetSavedTheme() {
            var themeName = Preferences.Get(ThemeKey, "System");
            return themeName switch {
                "Light" => AppTheme.Light,
                "Dark" => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };
        }

        public void SetTheme(AppTheme theme) {
            var themeName = theme switch {
                AppTheme.Light => "Light",
                AppTheme.Dark => "Dark",
                _ => "System"
            };

            Preferences.Set(ThemeKey, themeName);
            
            if (Application.Current != null) {
                Application.Current.UserAppTheme = theme;
            }
        }

        public string GetThemeName() {
            return Preferences.Get(ThemeKey, "System");
        }
    }
}
