using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Reporting;
using Moeen.Api.Shared.Responses.Reporting;
using System.Threading.Tasks;

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

        [HttpPost("attendance")]
        public async Task<ActionResult<ReportDto>> GenerateAttendanceReport(GenerateAttendanceReportRequest request)
        {
            var result = await _reportingService.GenerateAttendanceReportAsync(request);
            return Ok(result);
        }

        [HttpPost("performance")]
        public async Task<ActionResult<ReportDto>> GeneratePerformanceReport(GeneratePerformanceReportRequest request)
        {
            var result = await _reportingService.GeneratePerformanceReportAsync(request);
            return Ok(result);
        }

        [HttpPost("template")]
        public async Task<ActionResult<CustomizeReportTemplateResponse>> CustomizeTemplate(CustomizeReportTemplateRequest request)
        {
            var result = await _reportingService.CustomizeReportTemplateAsync(request);
            return Ok(result);
        }

        [HttpPost("schedule")]
        public async Task<ActionResult<SchedulePeriodicReportResponse>> ScheduleReport(SchedulePeriodicReportRequest request)
        {
            var result = await _reportingService.SchedulePeriodicReportAsync(request);
            return Ok(result);
        }

        [HttpPost("share")]
        public async Task<ActionResult<ShareReportResponse>> ShareReport(ShareReportRequest request)
        {
            var result = await _reportingService.ShareReportAsync(request);
            return Ok(result);
        }

        [HttpPost("archive")]
        public async Task<ActionResult<ArchiveReportResponse>> ArchiveReport(ArchiveReportRequest request)
        {
            var result = await _reportingService.ArchiveReportAsync(request);
            return Ok(result);
        }

        [HttpPost("compare")]
        public async Task<ActionResult<ComparisonReportDto>> CompareReports(CompareReportsRequest request)
        {
            var result = await _reportingService.CompareReportsAsync(request);
            return Ok(result);
        }
    }
}