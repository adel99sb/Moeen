using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Goal;
using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/goal-tracking")]
    [ApiController]
    public class GoalTrackingController : ControllerBase
    {
        private readonly IGoalService _goalService;

        public GoalTrackingController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        [HttpPost("daily-entry")]
        public async Task<ActionResult<GeneralResponse>> RecordDailyEntry([FromBody] RecordDailyEntryRequest request)
            => Ok(await _goalService.RecordDailyEntryAsync(request));

        [HttpPut("records/{recordId:guid}")]
        public async Task<ActionResult<GeneralResponse>> UpdateProgressRecord(
            [FromRoute] Guid recordId,
            [FromBody] UpdateProgressRecordRequest request)
        {
            request.RecordId = recordId;
            return Ok(await _goalService.UpdateProgressRecordAsync(request));
        }

        [HttpGet("students/{studentId:guid}/summary")]
        public async Task<ActionResult<GeneralResponse>> GetStudentProgressSummary(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
            => Ok(await _goalService.GetStudentProgressSummaryAsync(new GetStudentProgressSummaryRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        [HttpGet("students/{studentId:guid}/history")]
        public async Task<ActionResult<GeneralResponse>> GetStudentProgressHistory(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] ProgressRecordType? recordType,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
            => Ok(await _goalService.GetStudentProgressHistoryAsync(new GetStudentProgressHistoryRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate,
                RecordType = recordType,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));

        [HttpGet("circle/overview")]
        public async Task<ActionResult<GeneralResponse>> GetCirclePerformanceOverview(
            [FromQuery] Guid? circleId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
            => Ok(await _goalService.GetCirclePerformanceOverviewAsync(new GetCirclePerformanceOverviewRequest
            {
                CircleId = circleId,
                FromDate = fromDate,
                ToDate = toDate
            }));
    }
}