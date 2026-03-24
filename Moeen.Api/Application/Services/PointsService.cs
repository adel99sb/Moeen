using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Points;
using Moeen.Api.Shared.Responses.Points;

namespace Moeen.Api.Application.Services
{
    public class PointsService : IPointsService
    {
        public Task<GetPointsHistoryResponse> GetPointsHistoryAsync(GetPointsHistoryRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GetStudentPointsResponse> GetStudentPointsAsync(GetStudentPointsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SetupPointsSystemResponse> SetupPointsSystemAsync(SetupPointsSystemRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
