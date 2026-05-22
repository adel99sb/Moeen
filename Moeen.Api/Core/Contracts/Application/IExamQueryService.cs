using Moeen.Shared.Requests.ExamQuery;
using Moeen.Shared.Responses;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IExamQueryService
    {
        /// <summary>
        /// الحصول على نتيجة اختبار بواسطة المعرف
        /// </summary>
        Task<GeneralResponse> GetExamResultByIdAsync(GetExamResultByIdRequest request);

        /// <summary>
        /// البحث في نتائج الاختبارات
        /// </summary>
        Task<GeneralResponse> SearchExamResultsAsync(SearchExamResultsRequest request);

        /// <summary>
        /// الحصول على اختبارات طالب معين
        /// </summary>
        Task<GeneralResponse> GetStudentExamsAsync(GetStudentExamsRequest request);

        /// <summary>
        /// [GET] جلب اختبارات حلقة معينة مع التصفح
        /// </summary>
        Task<GeneralResponse> GetExamsByHalqaAsync(GetExamsByHalqaRequest request);

        /// <summary>
        /// [GET] جلب اختبارات طالب معين ضمن فترة زمنية
        /// </summary>
        Task<GeneralResponse> GetStudentExamsByDateRangeAsync(GetStudentExamsByDateRequest request);

        /// <summary>
        /// [GET] جلب اختبارات معلم معين
        /// </summary>
        Task<GeneralResponse> GetExamsByTeacherAsync(GetExamsByTeacherRequest request);

        ///// <summary>
        ///// [GET] جلب اختبارات حسب المرحلة
        ///// </summary>
        //Task<GeneralResponse> GetExamsByPhaseAsync(GetExamsByPhaseRequest request);

        /// <summary>
        /// [GET] إحصائيات شاملة للاختبارات
        /// </summary>
        Task<GeneralResponse> GetExamStatisticsAsync(GetExamStatisticsRequest request);

        /// <summary>
        /// [GET] تحليلات اختبارات حلقة
        /// </summary>
        Task<GeneralResponse> GetHalqaExamAnalyticsAsync(GetHalqaAnalyticsRequest request);

        /// <summary>
        /// [POST] مقارنة أداء الحلقات
        /// </summary>
        Task<GeneralResponse> CompareHalqasPerformanceAsync(CompareHalqasRequest request);

        /// <summary>
        /// [POST] تجهيز بيانات الاختبارات للتصدير
        /// </summary>
        Task<GeneralResponse> PrepareExamDataForExportAsync(PrepareExportRequest request);

        /// <summary>
        /// جلب قائمة الطلاب الأكثر تميزاً في الاختبارات (حسب النقاط أو الدرجات)
        /// </summary>
        Task<GeneralResponse> GetTopPerformingStudentsInExamsAsync(GetTopPerformingStudentsInExamsRequest request);

        /// <summary>
        /// جلب قائمة الطلاب الأقل تميزاً (المتعثرين) في الاختبارات
        /// </summary>
        Task<GeneralResponse> GetLowestPerformingStudentsInExamsAsync(GetLowestPerformingStudentsInExamsRequest request);

        /// <summary>
        /// إحصائيات عامة للمختبر (نسبة النجاح، نسبة الإعادة، إجمالي النقاط، إلخ)
        /// </summary>
        Task<GeneralResponse> GetLabStatisticsAsync(GetLabStatisticsRequest request);
    }
}