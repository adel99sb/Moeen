namespace Moeen.App.Infrastructure.Http
{
    public static class ApiRoutes
    {
#if ANDROID
        public static string BaseUrl { get; } = "http://10.0.2.2:5055/";
#else
        public static string BaseUrl { get; } = "https://localhost:7023/";
#endif

        // Auth
        public static string LoginRoute { get; } = "api/User/login";
        
        // Content Sharing
        public static string GetAllContentSharingRoute { get; }
            = "api/ContentSharing/all";
        public static string InteractWithPostRoute { get; } = "api/ContentSharing/interact";
    }
}
