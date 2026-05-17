namespace Moeen.Dashboard.Infrastructure.Http
{
    public static class ApiRoutes
    {
        public static string BaseUrl { get; } = "https://localhost:7023/";
        public static string LoginRoute { get; } = "api/User/login";
        public static string registerRoute { get; } = "api/User/register";
   
        public static string SearchRoute { get; } = "api/User/search";
        public static string SendVerifyEmailRoute { get; } = "api/User/send-verify-email-code";
        public static string VerifyEmailRoute { get; } = "api/User/verify-email";
        public static string GetPostRoute { get; } = "api/ContentSharing/posts/{postId}";
        public static string GetPostInterractionRoute { get; } = "api/ContentSharing/posts/{postId}/interactions";
        public static string DeletePostRoute { get; } = "api/ContentSharing/posts/{postId}";
    }
}




