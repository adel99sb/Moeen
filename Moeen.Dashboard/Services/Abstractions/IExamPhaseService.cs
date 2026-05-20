using Moeen.Shared.Requests.ExamPhase;
using Moeen.Shared.Responses.ExamPhase;

namespace Moeen.Dashboard.Application.Services.Abstractions
{
    public interface IExamPhaseService
    {
        Task<ExamPhaseDto> DefineExamPhaseAsync(DefineExamPhaseRequest request);
        Task<GetExamPhasesResponse> GetExamPhasesAsync(GetExamPhasesRequest request);
        Task<ExamPhaseDto> GetExamPhaseByIdAsync(GetExamPhaseByIdRequest request);
        Task<List<ExamPhaseDto>> GetExamPhasesByCircleAsync(GetExamPhasesByCircleRequest request);
        Task<ExamPhaseDto> UpdateExamPhaseInfoAsync(UpdateExamPhaseInfoRequest request);
        Task<DeleteExamPhaseResponse> DeleteExamPhaseAsync(DeleteExamPhaseRequest request);
    }
}
