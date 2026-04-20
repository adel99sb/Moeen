using Moeen.Api.Shared.Requests.Reporting;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.Reporting;
using System;
using System.Collections.Generic;
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
        /// مشاركة تقرير
        /// </summary>
        Task<ShareReportResponse> ShareReportAsync(ShareReportRequest request);

        /// <summary>
        /// أرشفة تقرير
        /// </summary>
        Task<ArchiveReportResponse> ArchiveReportAsync(ArchiveReportRequest request);

        /// <summary>
        /// مقارنة تقارير
        /// </summary>
        Task<ComparisonReportDto> CompareReportsAsync(CompareReportsRequest request);

        /// <summary>
        /// إنشاء تقرير حضور مباشر عبر معرف الحلقة
        /// </summary>
        Task<ReportDto> GenerateAttendanceReportByCircleIdAsync(Guid circleId);

        /// <summary>
        /// [GET] جلب تقرير محدد بمعرفه مع تفاصيله الكاملة
        /// </summary>
        Task<ReportDto> GetReportByIdAsync(GetReportByIdRequest request);

        /// <summary>
        /// [GET] جلب التقارير التابعة لطالب معين
        /// </summary>
        Task<List<ReportDto>> GetReportsByStudentAsync(GetReportsByStudentRequest request);

        /// <summary>
        /// [GET] جلب التقارير التابعة لمعلم/مسؤول معين
        /// </summary>
        Task<List<ReportDto>> GetReportsByUserAsync(GetReportsByUserRequest request);

        /// <summary>
        /// [PUT] تحديث عنوان/وصف تقرير موجود
        /// </summary>
        Task<ReportDto> UpdateReportInfoAsync(UpdateReportInfoRequest request);

        /// <summary>
        /// [DELETE] حذف تقرير نهائيًا
        /// </summary>
        Task<OperationResponseDto> DeleteReportAsync(DeleteReportRequest request);
    }
}