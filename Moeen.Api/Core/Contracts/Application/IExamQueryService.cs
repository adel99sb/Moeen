using Moeen.Api.Shared.Requests.ExamQuery;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.ExamCommand;
using Moeen.Api.Shared.Responses.ExamQuery;
using System.Collections.Generic;
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

        /// <summary>
        /// [GET] جلب نتائج اختبارات حلقة معينة مع دعم التصفح والتصفية
        /// </summary>
        Task<PagedList<ExamResultDto>> GetExamsByCircleAsync(GetExamsByCircleRequest request);

        /// <summary>
        /// [GET] جلب اختبارات طالب ضمن فترة زمنية محددة
        /// </summary>
        Task<List<ExamResultDto>> GetStudentExamsByDateRangeAsync(GetStudentExamsByDateRequest request);

        /// <summary>
        /// [GET] جلب اختبارات معلم معين
        /// </summary>
        Task<List<ExamResultDto>> GetExamsByTeacherAsync(GetExamsByTeacherRequest request);

        /// <summary>
        /// [GET] جلب الاختبارات حسب مرحلة اختبارية محددة
        /// </summary>
        Task<List<ExamResultDto>> GetExamsByPhaseAsync(GetExamsByPhaseRequest request);

        /// <summary>
        /// [GET] الحصول على إحصائيات شاملة للاختبارات
        /// </summary>
        Task<ExamStatisticsDto> GetExamStatisticsAsync(GetExamStatisticsRequest request);

        /// <summary>
        /// [GET] جلب إحصائيات أداء الطلاب في حلقة معينة
        /// </summary>
        Task<CircleExamAnalyticsDto> GetCircleExamAnalyticsAsync(GetCircleAnalyticsRequest request);

        /// <summary>
        /// [GET] مقارنة أداء الحلقات في الاختبارات
        /// </summary>
        Task<CircleComparisonDto> CompareCirclesPerformanceAsync(CompareCirclesRequest request);

        /// <summary>
        /// [GET] تجهيز بيانات الاختبارات للتصدير
        /// </summary>
        Task<ExportExamDataDto> PrepareExamDataForExportAsync(PrepareExportRequest request);
    }
}