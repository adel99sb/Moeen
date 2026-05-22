using Moeen.Shared.Requests.ExamCommand;
using Moeen.Shared.Responses;
using System.Threading.Tasks;
namespace Moeen.Api.Core.Contracts.Application
{
    public interface IExamCommandService
    {
        /// <summary>
        /// تسجيل اختبار جديد مع حساب التقدير والنقاط تلقائياً
        /// </summary>
        Task<GeneralResponse> RegisterExamAsync(RegisterExamRequest request);

        /// <summary>
        /// تحديث نتيجة اختبار (إذا كان هناك خطأ)
        /// </summary>
        Task<GeneralResponse> UpdateExamResultAsync(UpdateExamResultRequest request);
            
        /// <summary>
        /// حذف نتيجة اختبار
        /// </summary>
        Task<GeneralResponse> DeleteExamResultAsync(DeleteExamResultRequest request);

        /// <summary>
        /// [PUT] تحديث البيانات الوصفية للاختبار (التاريخ، النوع، الملاحظات)
        /// </summary>
        Task<GeneralResponse> UpdateExamInfoAsync(UpdateExamInfoRequest request);

        /// <summary>
        /// [PUT] إضافة ملاحظات تقييمية أو توصيات للطالب بعد الاختبار
        /// </summary>
        Task<GeneralResponse> AddExamFeedbackAsync(AddExamFeedbackRequest request);

        /// <summary>
        /// إضافة اختبار جديد من قبل المختبر مع تفاصيل الأجزاء والتقدير
        /// </summary>
        Task<GeneralResponse> CreateLabExamAsync(CreateLabExamRequest request);
    }
}