using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Reporting;
using Moeen.Shared.Responses;
using System;
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

        [HttpGet("dashboard/indicators")]
        public async Task<ActionResult<GeneralResponse>> GetIndicators([FromQuery] GetGeneralPerformanceIndicatorsRequest request)
            => Ok(await _reportingService.GetGeneralPerformanceIndicatorsAsync(request));

        [HttpGet("circles/monthly-performance")]
        public async Task<ActionResult<GeneralResponse>> GetMonthlyPerformance([FromQuery] GetMonthlyCirclePerformanceRequest request)
            => Ok(await _reportingService.GetMonthlyCirclePerformanceAsync(request));

        [HttpGet("students/{studentId:guid}/progress")]
        public async Task<ActionResult<GeneralResponse>> GetStudentProgress(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string? type,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
            => Ok(await _reportingService.GetStudentProgressTimelineAsync(new GetStudentProgressTimelineRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate,
                Type = type,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));
    }
}