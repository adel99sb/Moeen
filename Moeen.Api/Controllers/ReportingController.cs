using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Reporting;
using Moeen.Api.Shared.Responses.Reporting;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingController : ControllerBase
    {
        private readonly IReportingService _reportingService;

        public ReportingController(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        /// <summary>
        /// POST (قديم/متوافق): تقرير الحضور.
        /// </summary>
        [HttpPost("attendance")]
        public async Task<ActionResult<ReportDto>> GenerateAttendanceReport(GenerateAttendanceReportRequest request)
            => Ok(await _reportingService.GenerateAttendanceReportAsync(request));

        /// <summary>
        /// GET (جديد): تقرير الحضور عبر CircleId.
        /// </summary>
        [HttpGet("attendance/{circleId:guid}")]
        public async Task<ActionResult<ReportDto>> GenerateAttendanceReportGet([FromRoute] Guid circleId)
            => Ok(await _reportingService.GenerateAttendanceReportAsync(new GenerateAttendanceReportRequest { CircleId = circleId }));

        /// <summary>
        /// POST (قديم/متوافق): تقرير الأداء.
        /// </summary>
        [HttpPost("performance")]
        public async Task<ActionResult<ReportDto>> GeneratePerformanceReport(GeneratePerformanceReportRequest request)
            => Ok(await _reportingService.GeneratePerformanceReportAsync(request));

        /// <summary>
        /// GET (جديد): تقرير الأداء عبر StudentId.
        /// </summary>
        [HttpGet("performance/{studentId:guid}")]
        public async Task<ActionResult<ReportDto>> GeneratePerformanceReportGet([FromRoute] Guid studentId)
            => Ok(await _reportingService.GeneratePerformanceReportAsync(new GeneratePerformanceReportRequest { StudentId = studentId }));

        [HttpPost("template")]
        public async Task<ActionResult<CustomizeReportTemplateResponse>> CustomizeTemplate(CustomizeReportTemplateRequest request)
            => Ok(await _reportingService.CustomizeReportTemplateAsync(request));

        [HttpPost("schedule")]
        public async Task<ActionResult<SchedulePeriodicReportResponse>> ScheduleReport(SchedulePeriodicReportRequest request)
            => Ok(await _reportingService.SchedulePeriodicReportAsync(request));

        [HttpPost("share")]
        public async Task<ActionResult<ShareReportResponse>> ShareReport(ShareReportRequest request)
            => Ok(await _reportingService.ShareReportAsync(request));

        [HttpPost("archive")]
        public async Task<ActionResult<ArchiveReportResponse>> ArchiveReport(ArchiveReportRequest request)
            => Ok(await _reportingService.ArchiveReportAsync(request));

        [HttpPost("compare")]
        public async Task<ActionResult<ComparisonReportDto>> CompareReports(CompareReportsRequest request)
            => Ok(await _reportingService.CompareReportsAsync(request));
    }
}   