namespace Moeen.App.Infrastructure.Http
{
    public static class ApiRoutes
    {
        public static string BaseUrl { get; } = "https://localhost:7023/";

        // Auth
        public static string LoginRoute { get; } = "api/User/login";
        
        // Content Sharing
        public static string GetAllContentSharingRoute { get; }
            = "api/ContentSharing/all";
    }
}
