using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.infrastructure.Data;
using Moeen.Api.Shared.Requests.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;

namespace Moeen.Api.Application.Services
{
    public class CircleTeacherAssignmentService : ICircleTeacherAssignmentService
    {
        public Task<OperationResponse> AssignTeacherToCircleAsync(AssignTeacherToCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponse> RemoveTeacherFromCircleAsync(RemoveTeacherFromCircleRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
