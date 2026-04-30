using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.CircleQuery;
using Moeen.Shared.Responses.Circle;
using Moeen.Shared.Responses.CircleQuery;

namespace Moeen.Api.Application.Services
{
    public class CircleQueryService : ICircleQueryService
    {
        public Task<CircleAttendanceReportResponse> GetCircleAttendanceReportAsync(GetCircleAttendanceReportRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CircleDto> GetCircleByIdAsync(GetCircleByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CircleStatisticsDto> GetCircleStatisticsAsync(GetCircleStatisticsRequest request)
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
