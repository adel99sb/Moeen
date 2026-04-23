using Moeen.Api.Shared.Requests.ExamCommand;
using Moeen.Api.Shared.Responses.ExamCommand;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IExamCommandService
    {
        /// <summary>
        /// تسجيل اختبار جديد مع حساب التقدير والنقاط تلقائياً
        /// </summary>
        Task<ExamResultDto> RegisterExamAsync(RegisterExamRequest request);

        /// <summary>
        /// تحديث نتيجة اختبار (إذا كان هناك خطأ)
        /// </summary>
        Task<ExamResultDto> UpdateExamResultAsync(UpdateExamResultRequest request);
            
        /// <summary>
        /// حذف نتيجة اختبار
        /// </summary>
        Task<DeleteExamResultResponse> DeleteExamResultAsync(DeleteExamResultRequest request);

        /// <summary>
        /// [PUT] تحديث البيانات الوصفية للاختبار (التاريخ، النوع، الملاحظات)
        /// </summary>
        Task<ExamResultDto> UpdateExamInfoAsync(UpdateExamInfoRequest request);

        /// <summary>
        /// [PUT] إضافة ملاحظات تقييمية أو توصيات للطالب بعد الاختبار
        /// </summary>
        Task<ExamFeedbackDto> AddExamFeedbackAsync(AddExamFeedbackRequest request);
    }
}