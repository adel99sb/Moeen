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
        /// <param name="request">بيانات الاختبار</param>
        /// <returns>نتيجة الاختبار المسجلة</returns>
        Task<ExamResultDto> RegisterExamAsync(RegisterExamRequest request);

        /// <summary>
        /// تحديث نتيجة اختبار (إذا كان هناك خطأ)
        /// </summary>
        /// <param name="request">معرف الاختبار والنتيجة الجديدة والملاحظات</param>
        /// <returns>نتيجة الاختبار بعد التحديث</returns>
        Task<ExamResultDto> UpdateExamResultAsync(UpdateExamResultRequest request);

        /// <summary>
        /// حذف نتيجة اختبار
        /// </summary>
        /// <param name="request">معرف الاختبار</param>
        /// <returns>حالة النجاح مع رسالة</returns>
        Task<DeleteExamResultResponse> DeleteExamResultAsync(DeleteExamResultRequest request);
    }
}