using Moeen.Shared.Requests.LessonManagement;
using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface ILessonManagementService
    {
        /// <summary>
        /// إنشاء درس جديد
        /// </summary>
        Task<GeneralResponse> CreateLessonAsync(CreateLessonRequest request);

        /// <summary>
        /// تحديث محتوى الدرس
        /// </summary>
        Task<GeneralResponse> UpdateLessonAsync(UpdateLessonRequest request);

        /// <summary>
        /// حذف درس
        /// </summary>
        Task<GeneralResponse> DeleteLessonAsync(DeleteLessonRequest request);

        /// <summary>
        /// [GET] جلب جميع الدروس التابعة لحلقة معينة مع دعم التصفح والتصفية
        /// </summary>
        Task<GeneralResponse> GetLessonsByCircleAsync(GetLessonsByCircleRequest request);

        /// <summary>
        /// [GET] جلب ملخص حول الدروس الأسبوعية
        /// </summary>
        Task<GeneralResponse> GetWeeklyLessonDashboardAsync(GetWeeklyLessonDashboardRequest request);

        /// <summary>
        /// [POST] تسجيل حضور الطلاب في درس معين
        /// </summary>
        Task<GeneralResponse> RecordLessonAttendanceAsync(RecordAttendanceRequest request);

        /// <summary>
        /// [GET] جلب تاريخ الدروس لطلاب حلقة معينة
        /// </summary>
        Task<GeneralResponse> GetLessonHistoryAsync(GetLessonHistoryRequest request);

        /// <summary>
        /// [GET] الحصول على نظرة عامة عن الحلقات
        /// </summary>
        Task<GeneralResponse> GetCirclesOverviewAsync(GetCirclesOverviewRequest request);

        /// <summary>
        /// [GET] الحصول على الدروس اليومية لطالب معين
        /// </summary>
        Task<GeneralResponse> GetStudentDailyLessonsAsync(GetStudentDailyLessonsRequest request);

        Task<GeneralResponse> GetManagedWeeklyLessonsAsync();
        Task<GeneralResponse> CreateWeeklyLessonAsync(CreateWeeklyLessonRequest request);
        Task<GeneralResponse> UpdateWeeklyLessonAsync(UpdateWeeklyLessonRequest request);
        Task<GeneralResponse> DeleteWeeklyLessonAsync(Guid weeklyLessonId);
        Task<GeneralResponse> CreateWeeklyLessonAssignmentAsync(CreateWeeklyLessonAssignmentRequest request);
        Task<GeneralResponse> UpdateWeeklyLessonAssignmentAsync(UpdateWeeklyLessonAssignmentRequest request);
        Task<GeneralResponse> DeleteWeeklyLessonAssignmentAsync(Guid assignmentId);
    }
}