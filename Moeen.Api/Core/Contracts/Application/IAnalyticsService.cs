using Moeen.Api.Core.Contracts;
using Moeen.Shared.Requests.Analytics;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Analytics;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IAnalyticsService
    {
        /// <summary>
        /// تحليل بيانات طالب معين
        /// </summary>
        Task<GeneralResponse> AnalyzeStudentDataAsync(AnalyzeStudentDataRequest request);

        /// <summary>
        /// تحليل أداء معلم معين
        /// </summary>
        Task<GeneralResponse> AnalyzeTeacherPerformanceAsync(AnalyzeTeacherPerformanceRequest request);

        /// <summary>
        /// تحليل فعالية حلقة معينة
        /// </summary>
        Task<GeneralResponse> AnalyzeHalqaEffectivenessAsync(AnalyzeHalqaEffectivenessRequest request);

        /// <summary>
        /// تحليل مباشر لبيانات طالب عبر المعرف
        /// </summary>
        Task<GeneralResponse> AnalyzeStudentDataByIdAsync(Guid studentId);

        /// <summary>
        /// تحليل مباشر لأداء معلم عبر المعرف
        /// </summary>      
        Task<GeneralResponse> AnalyzeTeacherPerformanceByIdAsync(Guid teacherId);

        /// <summary>
        /// تحليل مباشر لفعالية حلقة عبر المعرف
        /// </summary>
        Task<GeneralResponse> AnalyzeHalqaEffectivenessByIdAsync(Guid HalqaId);

        /// <summary>
        /// حفظ تقرير تحليلي لاستخدامه لاحقاً
        /// </summary>
        Task<GeneralResponse> SaveAnalyticsReportAsync(SaveAnalyticsReportRequest request);

        /// <summary>
        /// استعراض تقرير تحليلي محفوظ بواسطة المعرف
        /// </summary>
        Task<GeneralResponse> GetAnalyticsReportByIdAsync(Guid reportId);

        /// <summary>
        /// جلب قائمة التقارير التحليلية المحفوظة مع التصفية والتصفح
        /// </summary>
        Task<GeneralResponse> GetAllAnalyticsReportsAsync(AnalyticsReportFilter filter);

        /// <summary>
        /// تحديث تقرير تحليلي محفوظ
        /// </summary>
        Task<GeneralResponse> UpdateAnalyticsReportAsync(Guid reportId, UpdateAnalyticsReportRequest request);

        /// <summary>
        /// حذف تقرير تحليلي محفوظ
        /// </summary>
        Task<GeneralResponse> DeleteAnalyticsReportAsync(Guid reportId);

        /// <summary>
        /// احضار احصائيات لوحة تحكم المعلم
        /// </summary>
        Task<GeneralResponse> GetTeacherDashboardStatisticsAsync(GetTeacherDashboardStatisticsRequest request);

        /// <summary>
        /// الحصول على أكثر الطلاب تراجعاً
        /// </summary>
        Task<GeneralResponse> GetMostRegressingStudentAsync(GetMostRegressingStudentRequest request);

        /// <summary>
        /// ملخص خطط التحفيز
        /// </summary>
        Task<GeneralResponse> GetMotivationPlansSummaryAsync(GetMotivationPlansSummaryRequest request);
    }
}