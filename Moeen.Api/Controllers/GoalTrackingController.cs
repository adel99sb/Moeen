using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Goal;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Goal;
using System;
using System.Collections.Generic;
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

        [HttpPost("daily")]
        public async Task<ActionResult<SetDailyGoalResponse>> SetDailyGoal([FromBody] SetDailyGoalRequest request)
            => Ok(await _goalService.SetDailyGoalAsync(request));

        [HttpGet("{goalId:guid}")]
        public async Task<ActionResult<DailyGoalDto>> GetGoalById([FromRoute] Guid goalId)
            => Ok(await _goalService.GetGoalByIdAsync(new GetGoalByIdRequest { GoalId = goalId }));

        [HttpGet("students/{studentId:guid}")]
        public async Task<ActionResult<PagedList<DailyGoalDto>>> GetGoalsByStudent(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
            => Ok(await _goalService.GetGoalsByStudentAsync(new GetGoalsByStudentRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));

        [HttpGet("students/{studentId:guid}/by-date")]
        public async Task<ActionResult<List<DailyGoalDto>>> GetGoalsByDateRange(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
            => Ok(await _goalService.GetGoalsByDateRangeAsync(new GetGoalsByDateRangeRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        [HttpGet("students/{studentId:guid}/progress-report")]
        public async Task<ActionResult<StudentProgressReportDto>> GetStudentProgressReport(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
            => Ok(await _goalService.GetStudentProgressReportAsync(new GetStudentProgressReportRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        [HttpGet("daily-achievement")]
        public async Task<ActionResult<CheckDailyGoalAchievedResponse>> CheckDailyGoalAchieved(
            [FromQuery] Guid studentId,
            [FromQuery] DateTime date)
            => Ok(await _goalService.CheckDailyGoalAchievedAsync(new CheckDailyGoalAchievedRequest
            {
                StudentId = studentId,
                Date = date
            }));

        [HttpPut("{goalId:guid}")]
        public async Task<ActionResult<DailyGoalDto>> UpdateDailyGoal([FromRoute] Guid goalId, [FromBody] UpdateDailyGoalRequest request)
        {
            request.GoalId = goalId;
            return Ok(await _goalService.UpdateDailyGoalAsync(request));
        }

        [HttpPut("{goalId:guid}/feedback")]
        public async Task<ActionResult<GoalFeedbackDto>> AddGoalFeedback([FromRoute] Guid goalId, [FromBody] AddGoalFeedbackRequest request)
        {
            request.GoalId = goalId;
            return Ok(await _goalService.AddGoalFeedbackAsync(request));
        }

        [HttpDelete("{goalId:guid}")]
        public async Task<ActionResult<OperationResponseDto>> DeleteGoal([FromRoute] Guid goalId)
            => Ok(await _goalService.DeleteGoalAsync(new DeleteGoalRequest { GoalId = goalId }));

        [HttpDelete("{goalId:guid}/cancel")]
        public async Task<ActionResult<OperationResponseDto>> CancelDailyGoal([FromRoute] Guid goalId, [FromQuery] string? reason)
            => Ok(await _goalService.CancelDailyGoalAsync(new CancelDailyGoalRequest
            {
                GoalId = goalId,
                Reason = reason
            }));
    }
}