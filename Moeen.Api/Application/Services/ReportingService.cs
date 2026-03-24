using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Reporting;
using Moeen.Api.Shared.Responses.Reporting;

namespace Moeen.Api.Application.Services
{
    public class ReportingService : IReportingService
    {
        public Task<ArchiveReportResponse> ArchiveReportAsync(ArchiveReportRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ComparisonReportDto> CompareReportsAsync(CompareReportsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CustomizeReportTemplateResponse> CustomizeReportTemplateAsync(CustomizeReportTemplateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ReportDto> GenerateAttendanceReportAsync(GenerateAttendanceReportRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ReportDto> GeneratePerformanceReportAsync(GeneratePerformanceReportRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SchedulePeriodicReportResponse> SchedulePeriodicReportAsync(SchedulePeriodicReportRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ShareReportResponse> ShareReportAsync(ShareReportRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
