using Moeen.Api.Shared.Requests.Review;
using Moeen.Api.Shared.Responses.Review;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IReviewService
    {
        /// <summary>
        /// تسجيل مراجعة صفحة (قديمة) لطالب
        /// </summary>
        /// <param name="request">بيانات الصفحة المراجعة</param>
        /// <returns>عدد النقاط المكتسبة</returns>
        Task<RecordReviewPageResponse> RecordReviewPageAsync(RecordReviewPageRequest request);

        /// <summary>
        /// تسجيل مراجعة جزء كامل
        /// </summary>
        /// <param name="request">بيانات مراجعة الجزء</param>
        /// <returns>نتيجة المراجعة</returns>
        Task<ReviewResultDto> RecordJuzReviewAsync(RecordJuzReviewRequest request);

        /// <summary>
        /// تسجيل مراجعة عدة أجزاء دفعة واحدة
        /// </summary>
        /// <param name="request">معرف الطالب وقائمة مراجعات الأجزاء</param>
        /// <returns>نتائج المراجعات مع إحصائيات</returns>
        Task<MultipleJuzReviewResponse> RecordMultipleJuzReviewAsync(RecordMultipleJuzReviewRequest request);
    }
}