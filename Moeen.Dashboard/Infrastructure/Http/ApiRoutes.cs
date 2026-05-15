namespace Moeen.Dashboard.Infrastructure.Http
{
    public static class ApiRoutes
    {
        public static string BaseUrl { get; } = "https://localhost:7023/";
        public static string LoginRoute { get; } = "api/User/login";
        public static string RegisterRoute { get; } = "api/User/register";
        public static string SearchRoute { get; } = "api/User/search";
        public static string SendVerifyEmailRoute { get; } = "api/User/send-verify-email-code";
        public static string VerifyEmailRoute { get; } = "api/User/verify-email";


    }
}
