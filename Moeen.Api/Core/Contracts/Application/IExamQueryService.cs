using Moeen.Shared.Requests.ExamQuery;
using Moeen.Shared.Responses;
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
        Task<GeneralResponse> GetExamResultByIdAsync(GetExamResultByIdRequest request);

        /// <summary>
        /// البحث عن نتائج اختبارات وفق معايير محددة
        /// </summary>
        /// <param name="request">معايير البحث مع دعم التصفح</param>
        /// <returns>قائمة النتائج مع العدد الكلي</returns>
        Task<GeneralResponse> SearchExamResultsAsync(SearchExamResultsRequest request);

        /// <summary>
        /// الحصول على جميع اختبارات طالب معين
        /// </summary>
        /// <param name="request">معرف الطالب</param>
        /// <returns>قائمة اختبارات الطالب مع العدد</returns>
        Task<GeneralResponse> GetStudentExamsAsync(GetStudentExamsRequest request);

        /// <summary>
        /// [GET] جلب نتائج اختبارات حلقة معينة مع دعم التصفح والتصفية
        /// </summary>
        Task<GeneralResponse> GetExamsByHalqaAsync(GetExamsByHalqaRequest request);

        /// <summary>
        /// [GET] جلب اختبارات طالب ضمن فترة زمنية محددة
        /// </summary>
        Task<GeneralResponse> GetStudentExamsByDateRangeAsync(GetStudentExamsByDateRequest request);

        /// <summary>
        /// [GET] جلب اختبارات معلم معين
        /// </summary>
        Task<GeneralResponse> GetExamsByTeacherAsync(GetExamsByTeacherRequest request);

        /// <summary>
        /// [GET] جلب الاختبارات حسب مرحلة اختبارية محددة
        /// </summary>
        Task<GeneralResponse> GetExamsByPhaseAsync(GetExamsByPhaseRequest request);

        /// <summary>
        /// [GET] الحصول على إحصائيات شاملة للاختبارات
        /// </summary>
        Task<GeneralResponse> GetExamStatisticsAsync(GetExamStatisticsRequest request);

        /// <summary>
        /// [GET] جلب إحصائيات أداء الطلاب في حلقة معينة
        /// </summary>
        Task<GeneralResponse> GetHalqaExamAnalyticsAsync(GetHalqaAnalyticsRequest request);

        /// <summary>
        /// [GET] مقارنة أداء الحلقات في الاختبارات
        /// </summary>
        Task<GeneralResponse> CompareHalqasPerformanceAsync(CompareHalqasRequest request);

        /// <summary>
        /// [GET] تجهيز بيانات الاختبارات للتصدير
        /// </summary>
        Task<GeneralResponse> PrepareExamDataForExportAsync(PrepareExportRequest request);
    }
}