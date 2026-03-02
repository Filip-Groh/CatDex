namespace CatDex.Constants;

public static class AppConstants
{
    public static class Api
    {
        public const string BaseUrl = "https://api.thecatapi.com/v1/";
        public const string ApiKey = "live_GFAzFIw9j9DjPMJV4cYjqkbxdcDtn5CBrlNL7H7CPwKpj3LcGPfTI7PBhGCKuEtD";
        public const int MaxRetries = 3;
        public const int BaseDelayMs = 1000;
        public const int DefaultPageLimit = 10;

        public static class Endpoints
        {
            public const string Breeds = "breeds";
            public const string Images = "images";
            public const string ImagesSearch = "images/search";
        }
    }

    public static class Database
    {
        public const string FileName = "catDb.db3";
        public const int CacheDurationDays = 1;
    }

    public static class Preferences
    {
        public const string StoreImagesKey = "store_images_preference";
    }

    public static class Zoom
    {
        public const double MaxScale = 4;
        public const double MinScale = 1;
        public const double InitialScale = 1;
    }

    public static class Routes
    {
        public const string DiscoverPage = nameof(Views.DiscoverPage);
        public const string SeenPage = nameof(Views.SeenPage);
        public const string CreatePage = nameof(Views.CreatePage);
        public const string FavoritePage = nameof(Views.FavoritePage);
        public const string SettingsPage = nameof(Views.SettingsPage);
        public const string CatDetailsPage = nameof(Views.CatDetailsPage);
        public const string FullScreenImagePage = nameof(Views.FullScreenImagePage);
    }

    public static class QueryParameters
    {
        public const string CatId = nameof(CatId);
        public const string ImageUrl = nameof(ImageUrl);
        public const string ImageData = nameof(ImageData);
    }

    public static class Files
    {
        public const string ImageFileNameFormat = "cat_{0:yyyyMMddHHmmss}.jpg";
        public const string CustomCatIdPrefix = "_custom_";
    }

    public static class Http
    {
        public const string AcceptHeader = "application/json";
        public const string ApiKeyHeader = "x-api-key";
    }

    public static class DefaultValues
    {
        public const int DefaultImageWidth = 500;
        public const int DefaultImageHeight = 500;
    }
}
