using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Scheduling;
using Moeen.Api.Shared.Responses.Scheduling;

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
    }
}
