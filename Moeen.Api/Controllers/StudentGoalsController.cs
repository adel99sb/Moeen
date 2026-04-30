using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Goal;
using Moeen.Shared.Responses.Goal;

namespace Moeen.Api.Controllers
{
    [Route("api/students/{studentId:guid}/goals")]
    [ApiController]
    public class StudentGoalsController : ControllerBase
    {
        private readonly IGoalService _goalService;

        public StudentGoalsController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        /// <summary>
        /// √„—:  ⁄ÌÌ‰ «·Âœ› «·ÌÊ„Ì ··ÿ«·».
        /// </summary>
        [HttpPost("daily")]
        public async Task<ActionResult<SetDailyGoalResponse>> SetDailyGoal(
            [FromRoute] Guid studentId,
            [FromBody] SetDailyGoalRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.StudentId = studentId;
            return Ok(await _goalService.SetDailyGoalAsync(request));
        }

        /// <summary>
        /// GET: Ã·» «·Œÿ… «·ÌÊ„Ì… ··ÿ«·».
        /// </summary>
        [HttpGet("daily-plan")]
        public async Task<ActionResult<DailyPlanDto>> GetDailyPlan(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime date)
        {
            if (date == default)
                return BadRequest("Date is required.");

            return Ok(await _goalService.GetDailyPlanAsync(new GetDailyPlanRequest
            {
                StudentId = studentId,
                Date = date
            }));
        }

        /// <summary>
        /// GET: «· Õﬁﬁ „‰  ÕﬁÌﬁ «·Âœ› «·ÌÊ„Ì.
        /// </summary>
        [HttpGet("daily-achievement")]
        public async Task<ActionResult<CheckDailyGoalAchievedResponse>> CheckDailyGoalAchieved(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime date)
        {
            if (date == default)
                return BadRequest("Date is required.");

            return Ok(await _goalService.CheckDailyGoalAchievedAsync(new CheckDailyGoalAchievedRequest
            {
                StudentId = studentId,
                Date = date
            }));
        }
    }
}