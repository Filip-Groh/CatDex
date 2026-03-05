using CatDex.Constants;
using CatDex.Data;
using CatDex.Services;
using CatDex.Services.Interfaces;
using CatDex.ViewModels;
using CatDex.Views;
using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CatDex {
    public static class MauiProgram {
        public static MauiApp CreateMauiApp() {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>();
            builder.UseMauiCommunityToolkit();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, AppConstants.Database.FileName);

            builder.Services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath};Mode=ReadWriteCreate;Cache=Shared;Pooling=True"));

            builder.Services.AddScoped<IDataService, DataService>();
            builder.Services.AddHttpClient<IApiService, ApiService>(client =>
            {
                client.BaseAddress = new Uri(AppConstants.Api.BaseUrl);
                client.DefaultRequestHeaders.Add("Accept", AppConstants.Http.AcceptHeader);
                client.DefaultRequestHeaders.Add(AppConstants.Http.ApiKeyHeader, AppConstants.Api.ApiKey);
            });

            builder.Services.AddScoped<ICatRepositoryService, CatRepositoryService>();
            builder.Services.AddSingleton<IFileSaverService, FileSaverService>();
            builder.Services.AddSingleton<IThemeService, ThemeService>();
            builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();

            builder.Services.AddSingleton<DiscoveryViewModel>();
            builder.Services.AddSingleton<DiscoverPage>();

            builder.Services.AddSingleton<SeenViewModel>();
            builder.Services.AddSingleton<SeenPage>();

            builder.Services.AddSingleton<CreateViewModel>();
            builder.Services.AddSingleton<CreatePage>();

            builder.Services.AddSingleton<FavoriteViewModel>();
            builder.Services.AddSingleton<FavoritePage>();

            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<SettingsPage>();

            builder.Services.AddTransient<CatDetailsViewModel>();
            builder.Services.AddTransient<CatDetailsPage>();

            builder.Services.AddTransient<FullScreenImageViewModel>();
            builder.Services.AddTransient<FullScreenImagePage>();

            builder.ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
