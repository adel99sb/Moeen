using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Goal;
using Moeen.Api.Shared.Responses.Goal;
using System.Threading.Tasks;

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
        /// تعيين هدف يومي للطالب
        /// </summary>
        [HttpPost("set-daily-goal")]
        public async Task<ActionResult<SetDailyGoalResponse>> SetDailyGoal(SetDailyGoalRequest request)
        {
            var result = await _goalService.SetDailyGoalAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// التحقق من تحقيق الهدف اليومي
        /// </summary>
        [HttpPost("check-achieved")]
        public async Task<ActionResult<CheckDailyGoalAchievedResponse>> CheckDailyGoalAchieved(CheckDailyGoalAchievedRequest request)
        {
            var result = await _goalService.CheckDailyGoalAchievedAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على الخطة اليومية للطالب
        /// </summary>
        [HttpPost("daily-plan")]
        public async Task<ActionResult<DailyPlanDto>> GetDailyPlan(GetDailyPlanRequest request)
        {
            var result = await _goalService.GetDailyPlanAsync(request);
            return Ok(result);
        }
    }
}