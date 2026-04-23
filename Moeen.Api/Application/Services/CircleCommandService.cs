using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Circle;
using Moeen.Api.Shared.Responses.Circle;

namespace Moeen.Api.Application.Services
{
    public class CircleCommandService : ICircleCommandService
    {
        public Task<CircleDto> CreateCircleAsync(CreateCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCircleAsync(DeleteCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CircleDto> MoveToFoujAsync(MoveCircleToFoujRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CircleDto> ReassignTeacherAsync(ReassignCircleTeacherRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CircleDto> UpdateCircleAsync(UpdateCircleRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
