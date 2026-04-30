using Moeen.Api.Core.Contracts;
using Moeen.Shared.Requests.Analytics;
using Moeen.Shared.Responses.Analytics;
using System;
using System.Collections.Generic;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IAnalyticsService
    {
        /// <summary>
        /// تحليل بيانات طالب معين
        /// </summary>
        Task<StudentAnalyticsDto> AnalyzeStudentDataAsync(AnalyzeStudentDataRequest request);

        /// <summary>
        /// تحليل أداء معلم معين
        /// </summary>
        Task<TeacherAnalyticsDto> AnalyzeTeacherPerformanceAsync(AnalyzeTeacherPerformanceRequest request);

        /// <summary>
        /// تحليل فعالية حلقة معينة
        /// </summary>
        Task<CircleAnalyticsDto> AnalyzeCircleEffectivenessAsync(AnalyzeCircleEffectivenessRequest request);

        /// <summary>
        /// تحليل مباشر لبيانات طالب عبر المعرف
        /// </summary>
        Task<StudentAnalyticsDto> AnalyzeStudentDataByIdAsync(Guid studentId);

        /// <summary>
        /// تحليل مباشر لأداء معلم عبر المعرف
        /// </summary>      
        Task<TeacherAnalyticsDto> AnalyzeTeacherPerformanceByIdAsync(Guid teacherId);

        /// <summary>
        /// تحليل مباشر لفعالية حلقة عبر المعرف
        /// </summary>
        Task<CircleAnalyticsDto> AnalyzeCircleEffectivenessByIdAsync(Guid circleId);

        /// <summary>
        /// حفظ تقرير تحليلي لاستخدامه لاحقاً
        /// </summary>
        Task<AnalyticsReportDto> SaveAnalyticsReportAsync(SaveAnalyticsReportRequest request);

        /// <summary>
        /// استعراض تقرير تحليلي محفوظ بواسطة المعرف
        /// </summary>
        Task<AnalyticsReportDto> GetAnalyticsReportByIdAsync(Guid reportId);

        /// <summary>
        /// جلب قائمة التقارير التحليلية المحفوظة مع التصفية والتصفح
        /// </summary>
        Task<PagedResult<AnalyticsReportSummaryDto>> GetAllAnalyticsReportsAsync(AnalyticsReportFilter filter);

        /// <summary>
        /// تحديث تقرير تحليلي محفوظ
        /// </summary>
        Task<AnalyticsReportDto> UpdateAnalyticsReportAsync(Guid reportId, UpdateAnalyticsReportRequest request);

        /// <summary>
        /// حذف تقرير تحليلي محفوظ
        /// </summary>
        Task<bool> DeleteAnalyticsReportAsync(Guid reportId);
    }
}