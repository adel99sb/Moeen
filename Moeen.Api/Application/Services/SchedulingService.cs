using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Scheduling;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Scheduling;

namespace Moeen.Api.Application.Services
{
    public class SchedulingService : ISchedulingService
    {
        public Task<AssignScheduleToTeacherResponse> AssignScheduleToTeacherAsync(AssignScheduleToTeacherRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ScheduleResponseDto> CreateCircleScheduleAsync(CreateCircleScheduleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ScheduleOperationResponse> DeleteScheduleAsync(DeleteScheduleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<ScheduleResponseDto>> GetAllSchedulesAsync(GetAllSchedulesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ScheduleResponseDto> GetCircleScheduleByIdAsync(GetCircleScheduleByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ScheduleResponseDto> UpdateCircleScheduleAsync(UpdateCircleScheduleRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
