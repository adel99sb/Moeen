using Moeen.Shared.Requests.Progress;
using Moeen.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IProgressService
    {
        Task<GeneralResponse> CreateProgressEntryAsync(CreateProgressEntryRequest createRequest);
        Task<GeneralResponse> UpdateProgressEntryAsync(Guid entryId, UpdateProgressEntryRequest updateRequest);
        Task<GeneralResponse> DeleteProgressEntryAsync(Guid entryId);
        Task<GeneralResponse> GetProgressEntryByIdAsync(Guid entryId);

        Task<GeneralResponse> GetStudentProgressHistoryAsync(Guid studentId);
        Task<GeneralResponse> GetTeacherEntriesByDateAsync(Guid teacherId, DateTime? date);
    }
}