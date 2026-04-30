using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Points;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Points;

namespace Moeen.Api.Application.Services
{
    public class PointsService : IPointsService
    {
        public Task<PointsTransactionDto> AwardPointsManuallyAsync(AwardPointsManualRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> DeletePointRuleAsync(DeletePointRuleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<StudentPointsSummaryDto>> GetCirclePointsSummaryAsync(GetCirclePointsSummaryRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<PointRuleDto>> GetPointRulesAsync(GetPointRulesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GetPointsHistoryResponse> GetPointsHistoryAsync(GetPointsHistoryRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<StudentLeaderboardDto>> GetPointsLeaderboardAsync(GetLeaderboardRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<PointTypeDto>> GetPointTypesAsync(GetPointTypesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GetStudentPointsResponse> GetStudentPointsAsync(GetStudentPointsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PointsTransactionDto> GetTransactionByIdAsync(GetTransactionByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SetupPointsSystemResponse> SetupPointsSystemAsync(SetupPointsSystemRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PointRuleDto> UpdatePointRuleAsync(UpdatePointRuleRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
