namespace Moeen.App.Infrastructure.Http
{
    public static class ApiRoutes
    {

        public static string BaseUrl { get; } = "http://moeen.somee.com/";

        // Auth
        public static string LoginRoute { get; } = "api/User/login";

        // Student Mobile Dashboard
        public static string StudentDashboardMeRoute { get; } = "api/mobile/student-dashboard/me";
        public static string StudentProgressMeRoute { get; } = "api/mobile/student-progress/me";
        public static string StudentProfileMeRoute { get; } = "api/mobile/student-profile/me";
        public static string StudentProfileNoteRoute { get; } = "api/mobile/student-profile/me/note";
        public static string LibraryBooksRoute { get; } = "api/library-management/books";
        public static string StudentPostsMeRoute { get; } = "api/mobile/student-posts/me";
        public static string StudentPostToggleLikeRoute(Guid postId) => $"api/mobile/student-posts/{postId}/toggle-like";
        public static string ParentDashboardMeRoute { get; } = "api/mobile/parent-dashboard/me";
        public static string ParentProgressMeRoute { get; } = "api/mobile/parent-progress/me";
        public static string ParentProfileMeRoute { get; } = "api/mobile/parent-profile/me";
        public static string ParentProfileNoteRoute { get; } = "api/mobile/parent-profile/me/note";
        public static string ParentLibraryBooksRoute { get; } = "api/mobile/parent-library/books";
        public static string ParentPostsMeRoute { get; } = "api/mobile/parent-posts/me";
        public static string ParentPostToggleLikeRoute(Guid postId) => $"api/mobile/parent-posts/{postId}/toggle-like";

        // Content Sharing
        public static string GetAllContentSharingRoute { get; }
            = "api/ContentSharing/all";
        public static string InteractWithPostRoute { get; } = "api/ContentSharing/interact";
    }
}
