using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Goal;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Goal;

namespace Moeen.Api.Application.Services
{
    public class GoalService : IGoalService
    {
        public Task<GoalFeedbackDto> AddGoalFeedbackAsync(AddGoalFeedbackRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> CancelDailyGoalAsync(CancelDailyGoalRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CheckDailyGoalAchievedResponse> CheckDailyGoalAchievedAsync(CheckDailyGoalAchievedRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> DeleteGoalAsync(DeleteGoalRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DailyPlanDto> GetDailyPlanAsync(GetDailyPlanRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DailyGoalDto> GetGoalByIdAsync(GetGoalByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<DailyGoalDto>> GetGoalsByDateRangeAsync(GetGoalsByDateRangeRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<DailyGoalDto>> GetGoalsByStudentAsync(GetGoalsByStudentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<StudentProgressReportDto> GetStudentProgressReportAsync(GetStudentProgressReportRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SetDailyGoalResponse> SetDailyGoalAsync(SetDailyGoalRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DailyGoalDto> UpdateDailyGoalAsync(UpdateDailyGoalRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
