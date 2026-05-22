using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Memorization;
using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/memorization")]
    [ApiController]
    public class MemorizationController : ControllerBase
    {
        private readonly IMemorizationService _memorizationService;

        public MemorizationController(IMemorizationService memorizationService)
        {
            _memorizationService = memorizationService;
        }

        [HttpPost("page")]
        public async Task<ActionResult<GeneralResponse>> RecordPage([FromBody] RecordPageMemorizationRequest request)
            => Ok(await _memorizationService.RecordNewPageMemorizationAsync(request));

        [HttpGet("students/{studentId:guid}/last-page")]
        public async Task<ActionResult<GeneralResponse>> GetLastPage([FromRoute] Guid studentId)
            => Ok(await _memorizationService.GetLastMemorizedPageAsync(new GetLastMemorizedPageRequest { StudentId = studentId }));

        [HttpGet("records")]
        public async Task<ActionResult<GeneralResponse>> GetRecord([FromQuery] Guid studentId, [FromQuery] int pageNumber)
            => Ok(await _memorizationService.GetMemorizationRecordAsync(new GetMemorizationRecordRequest
            {
                StudentId = studentId,
                PageNumber = pageNumber
            }));

        [HttpGet("students/{studentId:guid}/history")]
        public async Task<ActionResult<GeneralResponse>> GetHistory(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
            => Ok(await _memorizationService.GetStudentMemorizationHistoryAsync(new GetStudentMemorizationHistoryRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));

        [HttpGet("students/{studentId:guid}/statistics")]
        public async Task<ActionResult<GeneralResponse>> GetStatistics(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
            => Ok(await _memorizationService.GetMemorizationStatisticsAsync(new GetMemorizationStatisticsRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        [HttpGet("circles/{circleId:guid}/progress")]
        public async Task<ActionResult<GeneralResponse>> GetCircleProgress([FromRoute] Guid circleId)
            => Ok(await _memorizationService.GetCircleMemorizationProgressAsync(new GetCircleProgressRequest { CircleId = circleId }));

        [HttpGet("students/{studentId:guid}/progress-report")]
        public async Task<ActionResult<GeneralResponse>> GetProgressReport(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
            => Ok(await _memorizationService.GetMemorizationProgressReportAsync(new GetProgressReportRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        [HttpPut("records/{recordId:guid}/grade")]
        public async Task<ActionResult<GeneralResponse>> UpdateGrade(
            [FromRoute] Guid recordId,
            [FromBody] UpdateMemorizationGradeRequest request)
        {
            request.RecordId = recordId;
            return Ok(await _memorizationService.UpdateMemorizationGradeAsync(request));
        }

        [HttpDelete("records/{recordId:guid}")]
        public async Task<ActionResult<GeneralResponse>> DeleteRecord([FromRoute] Guid recordId)
            => Ok(await _memorizationService.DeleteMemorizationRecordAsync(new DeleteMemorizationRecordRequest { RecordId = recordId }));

        [HttpGet("students/top-performing")]
        public async Task<ActionResult<GeneralResponse>> GetTopPerforming([FromQuery] GetTopPerformingStudentsRequest request)
            => Ok(await _memorizationService.GetTopPerformingStudentsAsync(request ?? new GetTopPerformingStudentsRequest()));

        [HttpGet("students/struggling")]
        public async Task<ActionResult<GeneralResponse>> GetStruggling([FromQuery] GetStrugglingStudentsRequest request)
            => Ok(await _memorizationService.GetStrugglingStudentsAsync(request ?? new GetStrugglingStudentsRequest()));
    }
}