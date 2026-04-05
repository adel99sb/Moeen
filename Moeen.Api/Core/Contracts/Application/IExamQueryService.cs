using Moeen.Api.Shared.Requests.ExamQuery;
using Moeen.Api.Shared.Responses.ExamCommand;
using Moeen.Api.Shared.Responses.ExamQuery;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IExamQueryService
    {
        /// <summary>
        /// الحصول على نتيجة اختبار محددة
        /// </summary>
        /// <param name="request">معرف الاختبار</param>
        /// <returns>نتيجة الاختبار</returns>
        Task<ExamResultDto> GetExamResultByIdAsync(GetExamResultByIdRequest request);

        /// <summary>
        /// البحث عن نتائج اختبارات وفق معايير محددة
        /// </summary>
        /// <param name="request">معايير البحث مع دعم التصفح</param>
        /// <returns>قائمة النتائج مع العدد الكلي</returns>
        Task<SearchExamResultsResponse> SearchExamResultsAsync(SearchExamResultsRequest request);

        /// <summary>
        /// الحصول على جميع اختبارات طالب معين
        /// </summary>
        /// <param name="request">معرف الطالب</param>
        /// <returns>قائمة اختبارات الطالب مع العدد</returns>
        Task<GetStudentExamsResponse> GetStudentExamsAsync(GetStudentExamsRequest request);
    }
}