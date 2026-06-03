using Moeen.Shared.Requests.Points;
using Moeen.Shared.Responses;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IPointsService
    {
        /// <summary>
        /// إعداد نظام النقاط
        /// </summary>
        Task<GeneralResponse> SetupPointsSystemAsync(SetupPointsSystemRequest request);

        /// <summary>
        /// الحصول على رصيد نقاط الطالب الحالي
        /// </summary>
        Task<GeneralResponse> GetStudentPointsAsync(GetStudentPointsRequest request);

        /// <summary>
        /// جلب لوحة الصدارة
        /// </summary>
        Task<GeneralResponse> GetPointsLeaderboardAsync(GetLeaderboardRequest request);

        /// <summary>
        /// منح نقاط يدوياً لطالب
        /// </summary>
        Task<GeneralResponse> AwardPointsManuallyAsync(AwardPointsManualRequest request);

        /// <summary>
        /// خصم نقاط من طالب
        /// </summary>
        Task<GeneralResponse> RemovePointsManuallyAsync(RemovePointsManualRequest request);

        /// <summary>
        /// إضافة النقاط التلقائية بناءً على القواعد الأسبوعية
        /// </summary>
        Task<GeneralResponse> EvaluateAutomaticPointsAsync(EvaluateAutomaticPointsRequest request);

        /// <summary>
        /// جلب تفصيل نقاط الطالب
        /// </summary>
        Task<GeneralResponse> GetStudentPointsBreakdownAsync(GetStudentPointsBreakdownRequest request);
    }
}