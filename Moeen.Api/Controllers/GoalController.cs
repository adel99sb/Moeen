using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Goal;
using Moeen.Api.Shared.Responses.Goal;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _goalService;

        public GoalController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        /// <summary>
        /// POST (قديم/متوافق): تعيين هدف يومي.
        /// </summary>
        [HttpPost("set-daily-goal")]
        public async Task<ActionResult<SetDailyGoalResponse>> SetDailyGoal(SetDailyGoalRequest request)
            => Ok(await _goalService.SetDailyGoalAsync(request));

        /// <summary>
        /// POST (قديم/متوافق): التحقق من تحقيق الهدف.
        /// </summary>
        [HttpPost("check-achieved")]
        public async Task<ActionResult<CheckDailyGoalAchievedResponse>> CheckDailyGoalAchieved(CheckDailyGoalAchievedRequest request)
            => Ok(await _goalService.CheckDailyGoalAchievedAsync(request));

        /// <summary>
        /// GET (جديد): التحقق من تحقيق الهدف عبر Query.
        /// </summary>
        [HttpGet("check-achieved")]
        public async Task<ActionResult<CheckDailyGoalAchievedResponse>> CheckDailyGoalAchievedGet([FromQuery] Guid studentId, [FromQuery] DateTime date)
            => Ok(await _goalService.CheckDailyGoalAchievedAsync(new CheckDailyGoalAchievedRequest
            {
                StudentId = studentId,
                Date = date
            }));

        /// <summary>
        /// POST (قديم/متوافق): جلب الخطة اليومية.
        /// </summary>
        [HttpPost("daily-plan")]    
        public async Task<ActionResult<DailyPlanDto>> GetDailyPlan(GetDailyPlanRequest request)
            => Ok(await _goalService.GetDailyPlanAsync(request));

        /// <summary>
        /// GET (جديد): جلب الخطة اليومية عبر Query.
        /// </summary>
        [HttpGet("daily-plan")]
        public async Task<ActionResult<DailyPlanDto>> GetDailyPlanGet([FromQuery] Guid studentId, [FromQuery] DateTime date)
            => Ok(await _goalService.GetDailyPlanAsync(new GetDailyPlanRequest
            {
                StudentId = studentId,
                Date = date
            }));
    }
}