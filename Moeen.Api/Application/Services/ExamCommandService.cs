using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamCommand;
using Moeen.Api.Shared.Responses.ExamCommand;

namespace Moeen.Api.Application.Services
{
    public class ExamCommandService : IExamCommandService
    {
        public Task<ExamFeedbackDto> AddExamFeedbackAsync(AddExamFeedbackRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteExamResultResponse> DeleteExamResultAsync(DeleteExamResultRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExamResultDto> RegisterExamAsync(RegisterExamRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExamResultDto> UpdateExamInfoAsync(UpdateExamInfoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExamResultDto> UpdateExamResultAsync(UpdateExamResultRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
