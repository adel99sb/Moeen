using Moeen.Shared.Requests.ExamGrading;
using Moeen.Shared.Responses.ExamGrading;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IExamGradingCriteriaService
    {
        /// <summary>
        /// إضافة أو تحديث معيار تقدير
        /// </summary>
        Task<GradingCriteriaDto> SetGradingCriteriaAsync(SetGradingCriteriaRequest request);

        /// <summary>
        /// الحصول على جميع معايير التقدير مع إمكانية التصفية حسب نوع الاختبار
        /// </summary>
        Task<GetGradingCriteriaResponse> GetGradingCriteriaAsync(GetGradingCriteriaRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل معيار تقدير محدد بمعرفه
        /// </summary>
        Task<GradingCriteriaDto> GetCriteriaByIdAsync(GetCriteriaByIdRequest request);

        /// <summary>
        /// حذف معيار تقدير (حذف نهائي)
        /// </summary>
        Task<DeleteGradingCriteriaResponse> DeleteGradingCriteriaAsync(DeleteGradingCriteriaRequest request);

    }
}