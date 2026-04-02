using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Registration;
using Moeen.Api.Shared.Responses.Registration;

namespace Moeen.Api.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        public Task<OperationResponse> RegisterInCircleAsync(RegisterInCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponse> TransferStudentAsync(TransferStudentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponse> UnregisterFromCircleAsync(UnregisterFromCircleRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
