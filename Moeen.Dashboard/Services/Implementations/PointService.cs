using System;
using System.Threading.Tasks;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Points;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementations

{
    public class PointsService : IPointsService
    {
        private readonly PointsApiClient _apiClient;

        public PointsService(PointsApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<GeneralResponse> SetupPointsSystemAsync(SetupPointsSystemRequest request)
        {
            return await _apiClient.SetupPointsSystemAsync(request);
        }

        public async Task<GeneralResponse> GetStudentPointsAsync(Guid studentId)
        {
            return await _apiClient.GetStudentPointsAsync(studentId);
        }

        public async Task<GeneralResponse> GetStudentPointsBreakdownAsync(GetStudentPointsBreakdownRequest request)
        {
            return await _apiClient.GetStudentPointsBreakdownAsync(request);
        }

        public async Task<GeneralResponse> GetPointsLeaderboardAsync(GetLeaderboardRequest request)
        {
            return await _apiClient.GetPointsLeaderboardAsync(request);
        }

        public async Task<GeneralResponse> AwardPointsManuallyAsync(AwardPointsManualRequest request)
        {
            return await _apiClient.AwardPointsManuallyAsync(request);
        }

        public async Task<GeneralResponse> RemovePointsManuallyAsync(RemovePointsManualRequest request)
        {
            return await _apiClient.RemovePointsManuallyAsync(request);
        }

        public async Task<GeneralResponse> EvaluateAutomaticPointsAsync(EvaluateAutomaticPointsRequest request)
        {
            return await _apiClient.EvaluateAutomaticPointsAsync(request);
        }
    }
}
