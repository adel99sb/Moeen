using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Goal;
using Moeen.Api.Shared.Responses.Goal;

namespace Moeen.Api.Application.Services
{
    public class GoalService : IGoalService
    {
        public Task<CheckDailyGoalAchievedResponse> CheckDailyGoalAchievedAsync(CheckDailyGoalAchievedRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DailyPlanDto> GetDailyPlanAsync(GetDailyPlanRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SetDailyGoalResponse> SetDailyGoalAsync(SetDailyGoalRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
