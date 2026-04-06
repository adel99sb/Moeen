using Moeen.Api.Shared.Requests.ExamPhase;
using Moeen.Api.Shared.Responses.ExamPhase;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IExamPhaseService
    {
        /// <summary>
        /// تعريف مرحلة اختبارية جديدة (مجموعة أجزاء)
        /// </summary>
        /// <param name="request">بيانات المرحلة</param>
        /// <returns>المرحلة المُنشأة</returns>
        Task<ExamPhaseDto> DefineExamPhaseAsync(DefineExamPhaseRequest request);

        /// <summary>
        /// الحصول على جميع المراحل المعرفة
        /// </summary>
        /// <param name="request">طلب يدعم التصفح</param>
        /// <returns>قائمة المراحل مع معلومات التصفح</returns>
        Task<GetExamPhasesResponse> GetExamPhasesAsync(GetExamPhasesRequest request);

        /// <summary>
        /// حذف مرحلة (إذا لم تكن مستخدمة)
        /// </summary>
        /// <param name="request">معرف المرحلة</param>
        /// <returns>نتيجة الحذف</returns>
        Task<DeleteExamPhaseResponse> DeleteExamPhaseAsync(DeleteExamPhaseRequest request);
    }
}