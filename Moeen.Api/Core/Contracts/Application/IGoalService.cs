using Moeen.Api.Shared.Requests.Goal;
using Moeen.Api.Shared.Responses.Goal;
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
    }
}