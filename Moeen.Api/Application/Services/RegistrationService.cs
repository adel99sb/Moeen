using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Registration;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Registration;

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
