using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Reporting;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Reporting;

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
        public async Task<ActionResult<ReportDto>> GenerateAttendanceReport([FromBody] GenerateAttendanceReportRequest request)
            => Ok(await _reportingService.GenerateAttendanceReportAsync(request));

        /// <summary>
        /// GET (جديد): تقرير الحضور عبر CircleId.
        /// </summary>
        [HttpGet("attendance/{circleId:guid}")]
        public async Task<ActionResult<ReportDto>> GenerateAttendanceReportGet([FromRoute] Guid circleId)
            => Ok(await _reportingService.GenerateAttendanceReportByCircleIdAsync(circleId));

        /// <summary>
        /// POST (قديم/متوافق): تقرير الأداء.
        /// </summary>
        [HttpPost("performance")]
        public async Task<ActionResult<ReportDto>> GeneratePerformanceReport([FromBody] GeneratePerformanceReportRequest request)
            => Ok(await _reportingService.GeneratePerformanceReportAsync(request));

        /// <summary>
        /// GET (جديد): تقرير الأداء عبر StudentId.
        /// </summary>
        [HttpGet("performance/{studentId:guid}")]
        public async Task<ActionResult<ReportDto>> GeneratePerformanceReportGet([FromRoute] Guid studentId)
            => Ok(await _reportingService.GeneratePerformanceReportAsync(new GeneratePerformanceReportRequest { StudentId = studentId }));

        [HttpPost("template")]
        public async Task<ActionResult<CustomizeReportTemplateResponse>> CustomizeTemplate([FromBody] CustomizeReportTemplateRequest request)
            => Ok(await _reportingService.CustomizeReportTemplateAsync(request));

        [HttpPost("schedule")]
        public async Task<ActionResult<SchedulePeriodicReportResponse>> ScheduleReport([FromBody] SchedulePeriodicReportRequest request)
            => Ok(await _reportingService.SchedulePeriodicReportAsync(request));

        [HttpPost("share")]
        public async Task<ActionResult<ShareReportResponse>> ShareReport([FromBody] ShareReportRequest request)
            => Ok(await _reportingService.ShareReportAsync(request));

        [HttpPost("archive")]
        public async Task<ActionResult<ArchiveReportResponse>> ArchiveReport([FromBody] ArchiveReportRequest request)
            => Ok(await _reportingService.ArchiveReportAsync(request));

        [HttpPost("compare")]
        public async Task<ActionResult<ComparisonReportDto>> CompareReports([FromBody] CompareReportsRequest request)
            => Ok(await _reportingService.CompareReportsAsync(request));

        /// <summary>
        /// GET: جلب تقرير محدد بمعرفه.
        /// </summary>
        [HttpGet("reports/{reportId:guid}")]
        public async Task<ActionResult<ReportDto>> GetReportById([FromRoute] Guid reportId)
            => Ok(await _reportingService.GetReportByIdAsync(new GetReportByIdRequest { ReportId = reportId }));

        /// <summary>
        /// GET: جلب تقارير طالب.
        /// </summary>
        [HttpGet("students/{studentId:guid}/reports")]
        public async Task<ActionResult<List<ReportDto>>> GetReportsByStudent(
            [FromRoute] Guid studentId,
            [FromQuery] string? type,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
            => Ok(await _reportingService.GetReportsByStudentAsync(new GetReportsByStudentRequest
            {
                StudentId = studentId,
                Type = type,
                FromDate = fromDate,
                ToDate = toDate
            }));

        /// <summary>
        /// GET: جلب تقارير مستخدم (معلم/مسؤول).
        /// </summary>
        [HttpGet("users/{userId:guid}/reports")]
        public async Task<ActionResult<List<ReportDto>>> GetReportsByUser(
            [FromRoute] Guid userId,
            [FromQuery] string? type,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
            => Ok(await _reportingService.GetReportsByUserAsync(new GetReportsByUserRequest
            {
                UserId = userId,
                Type = type,
                FromDate = fromDate,
                ToDate = toDate
            }));

        /// <summary>
        /// PUT: تحديث معلومات تقرير.
        /// </summary>
        [HttpPut("reports/{reportId:guid}")]
        public async Task<ActionResult<ReportDto>> UpdateReportInfo(
            [FromRoute] Guid reportId,
            [FromBody] UpdateReportInfoRequest request)
        {
            request.ReportId = reportId;
            return Ok(await _reportingService.UpdateReportInfoAsync(request));
        }

        /// <summary>
        /// DELETE: حذف تقرير.
        /// </summary>
        [HttpDelete("reports/{reportId:guid}")]
        public async Task<ActionResult<OperationResponseDto>> DeleteReport([FromRoute] Guid reportId)
            => Ok(await _reportingService.DeleteReportAsync(new DeleteReportRequest { ReportId = reportId }));
    }
}