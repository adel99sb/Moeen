using Moeen.Shared.Requests.Goal;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Goal;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IGoalService
    {
        /// <summary>
        /// تعيين هدف يومي للطالب
        /// </summary>
        /// <param name="request">معرف الطالب وبيانات الهدف</param>
        /// <returns>حالة النجاح</returns>
        Task<SetDailyGoalResponse> SetDailyGoalAsync(SetDailyGoalRequest request);

        /// <summary>
        /// التحقق من تحقيق الهدف اليومي للطالب
        /// </summary>
        /// <param name="request">معرف الطالب والتاريخ</param>
        /// <returns>نتيجة التحقق</returns>
        Task<CheckDailyGoalAchievedResponse> CheckDailyGoalAchievedAsync(CheckDailyGoalAchievedRequest request);

        /// <summary>
        /// الحصول على المهام المقررة للطالب في يوم معين
        /// </summary>
        /// <param name="request">معرف الطالب والتاريخ</param>
        /// <returns>الخطة اليومية</returns>
        Task<DailyPlanDto> GetDailyPlanAsync(GetDailyPlanRequest request);

        /// <summary>
        /// [GET] جلب جميع الأهداف اليومية لطالب معين مع التصفح والتصفية
        /// </summary>
        Task<PagedList<DailyGoalDto>> GetGoalsByStudentAsync(GetGoalsByStudentRequest request);

        /// <summary>
        /// [GET] جلب تقرير تقدم الطالب بناءً على الأهداف المحققة
        /// </summary>
        Task<StudentProgressReportDto> GetStudentProgressReportAsync(GetStudentProgressReportRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل هدف يومي محدد بمعرفه
        /// </summary>
        Task<DailyGoalDto> GetGoalByIdAsync(GetGoalByIdRequest request);

        /// <summary>
        /// [GET] جلب أهداف طالب ضمن فترة زمنية محددة
        /// </summary>
        Task<List<DailyGoalDto>> GetGoalsByDateRangeAsync(GetGoalsByDateRangeRequest request);

        /// <summary>
        /// [PUT] تحديث تفاصيل هدف يومي
        /// </summary>
        Task<DailyGoalDto> UpdateDailyGoalAsync(UpdateDailyGoalRequest request);

        /// <summary>
        /// [PUT] إضافة ملاحظة أو تعليق من المعلم على هدف الطالب
        /// </summary>
        Task<GoalFeedbackDto> AddGoalFeedbackAsync(AddGoalFeedbackRequest request);

        /// <summary>
        /// [DELETE] حذف هدف نهائيًا
        /// </summary>
        Task<OperationResponseDto> DeleteGoalAsync(DeleteGoalRequest request);

        /// <summary>
        /// [DELETE] إلغاء هدف يومي معين
        /// </summary>
        Task<OperationResponseDto> CancelDailyGoalAsync(CancelDailyGoalRequest request);
    }
}