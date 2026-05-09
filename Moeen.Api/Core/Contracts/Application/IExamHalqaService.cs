using Moeen.Api.Shared.Requests.Exam_Halqa;
using Moeen.Api.Shared.Responses.Exam_Halqa;
using Moeen.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IExamHalqaService
    {
        // <summary>
        /// إنشاء أستاذ اختبارات جديد
        /// </summary>
        /// <param name="request">بيانات أستاذ الاختبار</param>
        /// <returns>استجابة تحتوي على نتيجة العملية وبيانات أستاذ الاختبار</returns>
        Task<GeneralResponse> CreateAsync(CreateExamTeacherRequest request);

        /// <summary>
        /// الحصول على بيانات أستاذ اختبار محدد
        /// </summary>
        /// <param name="request">معرف أستاذ الاختبار</param>
        /// <returns>استجابة تحتوي على بيانات أستاذ الاختبار</returns>
        Task<GeneralResponse> GetByIdAsync(GetExamTeacherByIdRequest request);

        /// <summary>
        /// ربط حلقة بأستاذ الاختبار
        /// </summary>
        /// <param name="request">معرف أستاذ الاختبار ومعرف الحلقة</param>
        /// <returns>استجابة تحتوي على نتيجة العملية</returns>
        Task<GeneralResponse> AssignHalqaAsync(AssignHalqaToExamTeacherRequest request);
    }
}
