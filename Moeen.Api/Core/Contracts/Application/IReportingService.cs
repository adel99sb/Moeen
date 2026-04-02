using Moeen.Api.Shared.Requests.Reporting;
using Moeen.Api.Shared.Responses.Reporting;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IReportingService
    {
        /// <summary>
        /// إنشاء تقرير الحضور لحلقة
        /// </summary>
        Task<ReportDto> GenerateAttendanceReportAsync(GenerateAttendanceReportRequest request);

        /// <summary>
        /// إنشاء تقرير أداء الحفظ لطالب
        /// </summary>
        Task<ReportDto> GeneratePerformanceReportAsync(GeneratePerformanceReportRequest request);

        /// <summary>
        /// تخصيص قوالب التقارير
        /// </summary>
        Task<CustomizeReportTemplateResponse> CustomizeReportTemplateAsync(CustomizeReportTemplateRequest request);

        /// <summary>
        /// جدولة تقارير دورية
        /// </summary>
        Task<SchedulePeriodicReportResponse> SchedulePeriodicReportAsync(SchedulePeriodicReportRequest request);

        /// <summary>
        /// مشاركة تقرير مع مستخدم آخر
        /// </summary>
        Task<ShareReportResponse> ShareReportAsync(ShareReportRequest request);

        /// <summary>
        /// أرشفة تقرير
        /// </summary>
        Task<ArchiveReportResponse> ArchiveReportAsync(ArchiveReportRequest request);

        /// <summary>
        /// مقارنة التقارير عبر فترة زمنية
        /// </summary>
        Task<ComparisonReportDto> CompareReportsAsync(CompareReportsRequest request);
    }
}