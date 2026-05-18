using Moeen.Shared.Requests.ExamCommand;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IExamCommandService
    {
        Task<GeneralResponse> RegisterExamAsync(RegisterExamRequest request);
        Task<GeneralResponse> AddExamFeedbackAsync(AddExamFeedbackRequest request);
        Task<GeneralResponse> UpdateExamInfoAsync(UpdateExamInfoRequest request);
        Task<GeneralResponse> UpdateExamResultAsync(UpdateExamResultRequest request);
        Task<GeneralResponse> DeleteExamResultAsync(DeleteExamResultRequest request);
    }
}
