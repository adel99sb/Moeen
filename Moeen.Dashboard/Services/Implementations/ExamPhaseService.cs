using Moeen.Dashboard.Application.Services.Abstractions;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Shared.Requests.ExamPhase;
using Moeen.Shared.Responses.ExamPhase;

namespace Moeen.Dashboard.Application.Services.Implementations
{
    public class ExamPhaseService : IExamPhaseService
    {
        private readonly ExamPhaseApiClient _client;

        public ExamPhaseService(ExamPhaseApiClient client)
        {
            _client = client;
        }

        public async Task<ExamPhaseDto> DefineExamPhaseAsync(DefineExamPhaseRequest request)
            => await _client.DefineExamPhaseAsync(request);

        public async Task<GetExamPhasesResponse> GetExamPhasesAsync(GetExamPhasesRequest request)
            => await _client.GetExamPhasesAsync(request);

        public async Task<ExamPhaseDto> GetExamPhaseByIdAsync(GetExamPhaseByIdRequest request)
            => await _client.GetExamPhaseByIdAsync(request);

        public async Task<List<ExamPhaseDto>> GetExamPhasesByCircleAsync(GetExamPhasesByCircleRequest request)
            => await _client.GetExamPhasesByCircleAsync(request);

        public async Task<ExamPhaseDto> UpdateExamPhaseInfoAsync(UpdateExamPhaseInfoRequest request)
            => await _client.UpdateExamPhaseInfoAsync(request);

        public async Task<DeleteExamPhaseResponse> DeleteExamPhaseAsync(DeleteExamPhaseRequest request)
            => await _client.DeleteExamPhaseAsync(request);
    }
}