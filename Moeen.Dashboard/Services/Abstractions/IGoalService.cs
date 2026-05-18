using System.Threading.Tasks;
using Moeen.Shared.Requests.Goal;
using Moeen.Shared.Responses;
namespace Moeen.Dashboard.Services.Abstractions

{
    public interface IGoalService
    {
        Task<GeneralResponse> RecordDailyEntryAsync(RecordDailyEntryRequest request);
        Task<GeneralResponse> UpdateProgressRecordAsync(UpdateProgressRecordRequest request);
        Task<GeneralResponse> GetStudentProgressSummaryAsync(GetStudentProgressSummaryRequest request);
        Task<GeneralResponse> GetStudentProgressHistoryAsync(GetStudentProgressHistoryRequest request);
        Task<GeneralResponse> GetCirclePerformanceOverviewAsync(GetCirclePerformanceOverviewRequest request);
    }
}
