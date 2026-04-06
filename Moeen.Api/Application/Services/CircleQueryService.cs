using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Circle;
using Moeen.Api.Shared.Responses.Circle;

namespace Moeen.Api.Application.Services
{
    public class CircleQueryService : ICircleQueryService
    {
        public Task<CircleDto> GetCircleByIdAsync(GetCircleByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CircleStudentsResponse> GetCircleStudentsAsync(GetCircleStudentsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CircleStudentsCountResponse> GetCircleStudentsCountAsync(GetCircleStudentsCountRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
