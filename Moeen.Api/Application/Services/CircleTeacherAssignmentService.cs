using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.infrastructure.Data;
using Moeen.Api.Shared.Requests.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;

namespace Moeen.Api.Application.Services
{
    public class CircleTeacherAssignmentService : ICircleTeacherAssignmentService
    {
        public Task<OperationResponseDto> AssignTeacherToCircleAsync(AssignTeacherToCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<CircleAssignmentDto>> GetCirclesByTeacherAsync(GetCirclesByTeacherRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<CircleAssignmentDto>> GetTeachersByCircleAsync(GetTeachersByCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> RemoveTeacherFromCircleAsync(RemoveTeacherFromCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> ReplaceTeacherInCircleAsync(ReplaceTeacherRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
