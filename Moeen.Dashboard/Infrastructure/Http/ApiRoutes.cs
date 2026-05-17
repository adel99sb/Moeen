namespace Moeen.Dashboard.Infrastructure.Http
{
    public static class ApiRoutes
    {
        public static string BaseUrl { get; } = "https://localhost:7023/";

        // Auth
        public static string LoginRoute { get; } = "api/User/login";
        public static string RegisterRoute { get; } = "api/User/register";

        // User
        public static string SearchUsersRoute { get; } = "api/User/search";
        public static string ChangeEmailRoute { get; } = "api/User/change-email";

        // Password
        public static string SendResetUrlRoute { get; } = "api/User/send-reset-url";
        public static string ResetPasswordRoute { get; } = "api/User/reset-password";

        // Verification
        public static string SendVerifyCodeRoute { get; } = "api/User/send-verify-code";
        public static string VerifyEmailRoute { get; } = "api/User/verify-email";

        // Dynamic
        public static string GetUserById(Guid id)
            => $"api/User/{id}";
    }
}