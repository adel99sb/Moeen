using Moeen.Api.Shared.Requests.ExamGrading;
using Moeen.Api.Shared.Responses.ExamGrading;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IExamGradingCriteriaService
    {
        /// <summary>
        /// إضافة أو تحديث معيار تقدير
        /// </summary>
        /// <param name="request">بيانات المعيار (إذا كان Id موجوداً فهذا تحديث)</param>
        /// <returns>المعيار المضاف/المحدث</returns>
        Task<GradingCriteriaDto> SetGradingCriteriaAsync(SetGradingCriteriaRequest request);

        /// <summary>
        /// الحصول على جميع معايير التقدير مع إمكانية التصفية حسب نوع الاختبار
        /// </summary>
        /// <param name="request">معايير التصفية (اختياري)</param>
        /// <returns>قائمة المعايير</returns>
        Task<GetGradingCriteriaResponse> GetGradingCriteriaAsync(GetGradingCriteriaRequest request);

        /// <summary>
        /// حذف معيار تقدير
        /// </summary>
        /// <param name="request">معرف المعيار</param>
        /// <returns>نتيجة الحذف</returns>
        Task<DeleteGradingCriteriaResponse> DeleteGradingCriteriaAsync(DeleteGradingCriteriaRequest request);
    }
}