using Moeen.Api.Shared.Requests.ExamPhase;
using Moeen.Api.Shared.Responses.ExamPhase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IExamPhaseService
    {
        /// <summary>
        /// تعريف مرحلة اختبارية جديدة (مجموعة أجزاء)
        /// </summary>
        Task<ExamPhaseDto> DefineExamPhaseAsync(DefineExamPhaseRequest request);

        /// <summary>
        /// الحصول على جميع المراحل المعرفة
        /// </summary>
        Task<GetExamPhasesResponse> GetExamPhasesAsync(GetExamPhasesRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل مرحلة اختبارية محددة بمعرفها
        /// </summary>
        Task<ExamPhaseDto> GetExamPhaseByIdAsync(GetExamPhaseByIdRequest request);

        /// <summary>
        /// [GET] جلب المراحل المرتبطة بحلقة معينة
        /// </summary>
        Task<List<ExamPhaseDto>> GetExamPhasesByCircleAsync(GetExamPhasesByCircleRequest request);

        /// <summary>
        /// [PUT] تحديث معلومات مرحلة اختبارية
        /// </summary>
        Task<ExamPhaseDto> UpdateExamPhaseInfoAsync(UpdateExamPhaseInfoRequest request);

        /// <summary>
        /// حذف مرحلة (إذا لم تكن مستخدمة)
        /// </summary>
        Task<DeleteExamPhaseResponse> DeleteExamPhaseAsync(DeleteExamPhaseRequest request);
    }
}