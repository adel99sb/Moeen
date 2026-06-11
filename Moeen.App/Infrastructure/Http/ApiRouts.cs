namespace Moeen.App.Infrastructure.Http
{
    public static class ApiRoutes
    {
        //public static string BaseUrl { get; } = "https://localhost:7023/";
        public static string BaseUrl { get; } = "http://192.168.43.178:5055";

        // Auth
        public static string LoginRoute { get; } = "api/User/login";
        
        // Content Sharing
        public static string GetAllContentSharingRoute { get; }
            = "api/ContentSharing/all";
        public static string InteractWithPostRoute { get; } = "api/ContentSharing/interact";

        // Daily Assignments
        // use string.Format(ApiRoutes.GetStudentDailyAssignmentsRoute, studentId)
        public static string GetStudentDailyAssignmentsRoute { get; }
            = "api/daily-assignments/students/{0}";

        // Library
        public static string GetAllBooksRoute { get; } = "api/library-management/books";
        public static string GetBookByIdRoute { get; } = "api/library-management/books/{0}";
        // User
        public static string GetUserByIdRoute { get; } = "api/User/{0}";
        // Points
        public static string GetStudentPointsRoute { get; } = "api/Points/students/{0}/points";
        public static string GetStudentPointsBreakdownRoute { get; } = "api/Points/students/{0}/points-breakdown";

        // Reporting
        public static string GetReportingIndicatorsRoute { get; } = "api/Reporting/dashboard/indicators";
        // Attendance
        public static string GetStudentAttendanceRateRoute { get; } = "api/Attendance/students/{0}/rate";
        public static string GetStudentAbsenceReportRoute { get; } = "api/Attendance/students/{0}/absence-report";
        // Enrollment
        public static string GetMemberProfileRoute { get; } = "api/enrollment/members/{0}";
        public static string GetChildrenByParentRoute { get; } = "api/enrollment/parents/{0}/children";

        // Feedback
        public static string SubmitComplaintRoute { get; } = "api/Feedback/complaint";
    }
}
