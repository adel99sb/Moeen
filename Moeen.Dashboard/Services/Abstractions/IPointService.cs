using Moeen.Shared.Requests.Points;
using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Dashboard.Services.Abstractions

{
    public interface IPointsService
    {
        Task<GeneralResponse> SetupPointsSystemAsync(SetupPointsSystemRequest request);
        Task<GeneralResponse> GetStudentPointsAsync(Guid studentId);
        Task<GeneralResponse> GetStudentPointsBreakdownAsync(GetStudentPointsBreakdownRequest request);
        Task<GeneralResponse> GetPointsLeaderboardAsync(GetLeaderboardRequest request);
        Task<GeneralResponse> AwardPointsManuallyAsync(AwardPointsManualRequest request);
        Task<GeneralResponse> RemovePointsManuallyAsync(RemovePointsManualRequest request);
        Task<GeneralResponse> EvaluateAutomaticPointsAsync(EvaluateAutomaticPointsRequest request);
    }
}
