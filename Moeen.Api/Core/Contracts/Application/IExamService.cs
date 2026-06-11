using Moeen.Shared.Requests.Exam;
using Moeen.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IExamService
    {
        Task<GeneralResponse> CreateExamAsync(CreateExamRequest createRequest);
        Task<GeneralResponse> UpdateExamAsync(Guid examId, UpdateExamRequest updateRequest);
        Task<GeneralResponse> DeleteExamAsync(Guid examId);
        Task<GeneralResponse> GetExamByIdAsync(Guid examId);

        Task<GeneralResponse> GetStudentExamsAsync(Guid studentId);
        Task<GeneralResponse> GetExamsByTeacherAndDateAsync(Guid teacherId, DateTime? date);
    }
}