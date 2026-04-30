using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.ExamPhase;
using Moeen.Shared.Responses.ExamPhase;

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

        public Task<ExamPhaseDto> GetExamPhaseByIdAsync(GetExamPhaseByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GetExamPhasesResponse> GetExamPhasesAsync(GetExamPhasesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExamPhaseDto>> GetExamPhasesByCircleAsync(GetExamPhasesByCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExamPhaseDto> UpdateExamPhaseInfoAsync(UpdateExamPhaseInfoRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
