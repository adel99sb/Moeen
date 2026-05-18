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
        ///
        //Post  
        public static string PublishPostRoute { get; } = "api/ContentSharing/publish";
        public static string ApdatePostRoute { get; } = "api/ContentSharing/posts/{postId}";
        public static string DeletePostRoute { get; } = "api/ContentSharing/posts/{postId}";
        public static string GetPostByIdRoute { get; } = "api/ContentSharing/posts/{postId}";
        public static string GetInteractionPostByIdRoute { get; } = "api/ContentSharing/posts/{postId}/interactions";
        public static string IntractPostRoute { get; } = "api/ContentSharing/interact";
        public static string ManagAnnoucmentRoute { get; } = "api/ContentSharing/manage-announcement";
        public static string DeleteOldRoute { get; } = "api/ContentSharing/delete-old";
        public static string AddMultiMediaRoute { get; } = "api/ContentSharing/add-multimedia";
        public static string SearchPostRoute { get; } = "api/ContentSharing/search";
        public static string GetSearchPostRoute { get; } = "api/ContentSharing/search";









    }
}