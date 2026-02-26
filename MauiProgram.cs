using CatDex.Data;
using CatDex.Services;
using CatDex.Services.Interfaces;
using CatDex.ViewModels;
using CatDex.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CatDex {
    public static class MauiProgram {
        public static MauiApp CreateMauiApp() {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "catDb.db3");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Filename={dbPath}"));

            builder.Services.AddHttpClient<IApiService, ApiService>(client =>
            {
                client.BaseAddress = new Uri("https://api.thecatapi.com/v1/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("x-api-key", "live_GFAzFIw9j9DjPMJV4cYjqkbxdcDtn5CBrlNL7H7CPwKpj3LcGPfTI7PBhGCKuEtD");
            });

            builder.Services.AddSingleton<ICatRepository, CatRepository>();

            builder.Services.AddSingleton<CatViewModel>();
            builder.Services.AddSingleton<DiscoverPage>();

            builder.Services.AddSingleton<SearchPage>();

            builder.Services.AddSingleton<CreatePage>();

            builder.Services.AddSingleton<FavoritePage>();

            builder.Services.AddSingleton<SettingsPage>();

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
