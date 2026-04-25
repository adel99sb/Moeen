using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Registration;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.Registration;

namespace Moeen.Api.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        public Task<PagedList<CircleStudentDto>> GetCircleStudentsAsync(GetCircleRegisteredStudentsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationDto> GetRegistrationByIdAsync(GetRegistrationByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> RegisterInCircleAsync(RegisterInCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> TransferStudentAsync(TransferStudentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> UnregisterFromCircleAsync(UnregisterFromCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationDto> UpdateRegistrationStatusAsync(UpdateRegistrationStatusRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
