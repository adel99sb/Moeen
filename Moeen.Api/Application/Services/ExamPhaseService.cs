using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamPhase;
using Moeen.Api.Shared.Responses.ExamPhase;

namespace Moeen.Api.Application.Services
{
    public class ExamPhaseService : IExamPhaseService
    {
        public Task<ExamPhaseDto> DefineExamPhaseAsync(DefineExamPhaseRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteExamPhaseResponse> DeleteExamPhaseAsync(DeleteExamPhaseRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GetExamPhasesResponse> GetExamPhasesAsync(GetExamPhasesRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
