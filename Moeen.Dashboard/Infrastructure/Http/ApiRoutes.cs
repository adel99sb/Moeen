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
        //exam command
        public static string RegisterExamRoute { get; } = "api/ExamCommand/register";
        public static string AddFeedbackOnExamRoute { get; } = "api/ExamCommand/add-feedback";
        public static string UpdateInfoExamRoute { get; } = "api/ExamCommand/update-info";
        public static string UpdateResultExamRoute { get; } = "api/ExamCommand/update-result";
        public static string DeleteResultExamRoute { get; } = "api/ExamCommand/delete";
        //exam halaqa
        public static string CreateExamTeacherRoute { get; } = "api/ExamHalqa/create";
        public static string GetExamTeacherByIdRoute { get; } = "api/ExamHalqa/get-by-id/{id}";
        public static string AssignHalqaToExamTeacherRoute { get; } = "api/ExamHalqa/assign-halqa";
        //enrollment
        public static string RegisterStudentAsyncRoute { get; } = "api/Enrollment/register-student";
        public static string AddTeacherAsyncRoute { get; } = "api/Enrollment/add-teacher";
        public static string RegisterParentAsyncRoute { get; } = "api/Enrollment/register-parent";
        public static string UpdateMemberInfoAsyncRoute { get; } = "api/Enrollment/update-member";
        public static string UpdateStudentInfoAsyncRoute { get; } = "api/Enrollment/students/update-info";
        public static string UpdateTeacherInfoAsyncRoute { get; } = "api/Enrollment/teachers/update-info";
        public static string UpdateParentInfoAsyncrRoute { get; } = "api/Enrollment/parents/update-info";
        public static string CancelMembershipAsyncRoute { get; } = "api/Enrollment/cancel-membership";
        public static string SearchMembersAsyncRoute { get; } = "api/Enrollment/search-members";
        public static string GetMemberProfileAsyncRoute { get; } = "api/Enrollment/member-profile";
        public static string UpdateMemberStatusAsyncRoute { get; } = "api/Enrollment/update-member-status";
        public static string GetAllStudentsAsyncRoute { get; } = "api/Enrollment/students";
        public static string GetAllTeachersAsyncRoute { get; } = "api/Enrollment/teachers";
        public static string GetAllParentsAsyncRoute { get; } = "api/Enrollment/parents";
        public static string GetAllSupervisorsAsyncRoute { get; } = "api/Enrollment/supervisors";
        public static string GetChildrenByParentAsyncRoute { get; } = "api/Enrollment/parents/{parentId}/children";
        public static string GetMemberStatisticsAsyncRoute { get; } = "api/Enrollment/statistics";
        public static string DeleteStudentAsyncRoute { get; } = "api/Enrollment/students/{studentId}";
        public static string DeleteTeacherAsyncRoute { get; } = "api/Enrollment/teachers/{teacherId}";
        public static string DeleteParentAsyncRoute { get; } = "api/Enrollment/parents/{parentId}";
        public static string ExportMembersListAsyncRoute { get; } = "api/Enrollment/export-members";
        












    }
}