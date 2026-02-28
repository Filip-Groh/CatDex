namespace CatDex.Services.Interfaces {
    public interface IThemeService {
        AppTheme GetSavedTheme();
        void SetTheme(AppTheme theme);
        string GetThemeName();
    }
}
