using System.Threading.Tasks;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Goal;
using Moeen.Shared.Responses;
namespace Moeen.Dashboard.Services.Implementations

{
    public class GoalService : IGoalService
    {
        private readonly GoalApiClient _apiClient;

        public GoalService(GoalApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<GeneralResponse> RecordDailyEntryAsync(RecordDailyEntryRequest request)
        {
            return await _apiClient.RecordDailyEntryAsync(request);
        }

        public async Task<GeneralResponse> UpdateProgressRecordAsync(UpdateProgressRecordRequest request)
        {
            return await _apiClient.UpdateProgressRecordAsync(request);
        }

        public async Task<GeneralResponse> GetStudentProgressSummaryAsync(GetStudentProgressSummaryRequest request)
        {
            return await _apiClient.GetStudentProgressSummaryAsync(request);
        }

        public async Task<GeneralResponse> GetStudentProgressHistoryAsync(GetStudentProgressHistoryRequest request)
        {
            return await _apiClient.GetStudentProgressHistoryAsync(request);
        }

        public async Task<GeneralResponse> GetCirclePerformanceOverviewAsync(GetHalqaPerformanceOverviewRequest request)
        {
            return await _apiClient.GetCirclePerformanceOverviewAsync(request);
        }
    }
}