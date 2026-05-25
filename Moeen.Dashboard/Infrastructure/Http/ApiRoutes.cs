namespace Moeen.Dashboard.Infrastructure.Http
{
    public static class ApiRoutes
    {
        public static string BaseUrl { get; } = "http://10.213.174.50:5055/";

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
        //exam phase
        public static string DefineExamPhaseAsyncRoute { get; } = "api/ExamPhase/define";
        public static string GetAllExamPhasesAsyncRoute { get; } = "api/ExamPhase";
        public static string GetExamPhaseByIdAsyncRoute { get; } = "api/ExamPhase/{phaseId}";
        public static string GetExamPhasesByCircleAsyncRoute { get; } = "api/ExamPhase/by-circle/{circleId}";
        public static string UpdateExamPhaseInfoAsyncRoute { get; } = "api/ExamPhase/{phaseId}";
        public static string DeleteExamPhaseAsyncRoute { get; } = "api/ExamPhase/{phaseId}";
        // exam query
        public static string GetExamResultByIdAsyncRoute { get; } = "api/ExamQuery/get-by-id";
        public static string SearchExamResultsAsyncRoute { get; } = "api/ExamQuery/search";
        public static string GetStudentExamsAsyncRoute { get; } = "api/ExamQuery/student-exams";
        public static string GetExamsByHalqaAsyncRoute { get; } = "api/ExamQuery/halqas/{halqaId}/exams";
        public static string GetStudentExamsByDateRangeAsyncRoute { get; } = "api/ExamQuery/students/{studentId}/exams/by-date";
        public static string GetExamsByTeacherAsyncRoute { get; } = "api/ExamQuery/teachers/{teacherId}/exams";
        public static string GetExamsByPhaseAsyncRoute { get; } = "api/ExamQuery/phases/{phaseId}/exams";
        public static string GetExamStatisticsAsyncRoute { get; } = "api/ExamQuery/statistics";
        public static string GetHalqaExamAnalyticsAsyncRoute { get; } = "api/ExamQuery/Halqa/{HalqaId}/analytics";
        public static string CompareHalqasPerformanceAsyncRoute { get; } = "api/ExamQuery/compare-Halqa";
        public static string PrepareExamDataForExportAsyncRoute { get; } = "api/ExamQuery/prepare-export";
        //feedback
        public static string SubmitComplaintAsyncRoute { get; } = "api/Feedback/complaint";
        public static string SubmitSuggestionAsyncRoute { get; } = "api/Feedback/suggestion";
        public static string ManageFeedbackAsyncRoute { get; } = "api/Feedback/manage";
        public static string UpdateComplaintStatusAsyncRoute { get; } = "api/Feedback/complaint/status";
        public static string UpdateSuggestionStatusAsyncRoute { get; } = "api/Feedback/suggestion/status";
        public static string GetComplaintsAsyncRoute { get; } = "api/Feedback/complaints";
        public static string GetSuggestionsAsyncRoute { get; } = "api/Feedback/complaints";
        //goal

        public static string RecordDailyEntryAsyncRoute { get; } = "api/goal-tracking/daily-entry";
        public static string UpdateProgressRecordAsyncRoute { get; } = "api/goal-tracking/records/{recordId}";
        public static string GetStudentProgressSummaryAsyncRoute { get; } = "api/goal-tracking/students/{studentId}/summary";
        public static string GetStudentProgressHistoryAsyncRoute { get; } = "api/goal-tracking/students/{studentId}/history";
        public static string GetCirclePerformanceOverviewAsyncRoute { get; } = "api/goal-tracking/circle/overview";
        //mosqu
        public static string AddMosquAsyncRoute { get; } = "api/Mosqu/add";
        public static string GetAllMosqusAsyncRoute { get; } = "api/Mosqu/all";
        public static string GetMosqueByIdAsyncRoute { get; } = "api/Mosqu/ById";
        public static string GetCirclesByMosqueAsyncRoute { get; } = "api/Mosqu/circles";
        public static string GetTeachersByMosqueAsyncRoute { get; } = "api/Mosqu/teachers";
        public static string GetMosqueStatisticsAsyncRoute { get; } = "api/Mosqu/statistics";
        public static string UpdateMosqueInfoAsyncRoute { get; } = "api/Mosqu/update";
        public static string AssignMosqueAdminAsyncRoute { get; } = "api/Mosqu/assign-admin";
        public static string DeleteMosqueAsyncRoute { get; } = "api/Mosqu/delete";
        public static string UnassignMosqueAdminAsyncRoute { get; } = "api/Mosqu/unassign-admin";
        //point
        public static string SetupPointsSystemAsyncRoute { get; } = "api/Points/setup";
        public static string GetStudentPointsAsyncRoute { get; } = "api/Points/students/{studentId}/points";
        public static string GetPointsLeaderboardAsyncRoute { get; } = "api/Points/leaderboard";
        public static string AwardPointsManuallyAsyncRoute { get; } = "api/Points/award";
        public static string RemovePointsManuallyAsyncRoute { get; } = "api/Points/remove";
        public static string EvaluateAutomaticPointsAsyncRoute { get; } = "api/Points/automatic-award";



        public static string GetUserByIdAsyncRoute { get; set; } = "api/User/{userId}";












    }
}