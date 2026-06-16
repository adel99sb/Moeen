using Moeen.Shared.Requests.LessonManagement;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.LessonManagement;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IWeeklyLessonService
    {
        Task<List<WeeklyLessonManagementDto>> GetWeeklyLessonsAsync();
        Task<GeneralResponse> CreateWeeklyLessonAsync(CreateWeeklyLessonRequest request);
        Task<GeneralResponse> UpdateWeeklyLessonAsync(Guid lessonId, UpdateWeeklyLessonRequest request);
        Task<GeneralResponse> DeleteWeeklyLessonAsync(Guid lessonId);
        Task<GeneralResponse> CreateWeeklyLessonRowAsync(Guid lessonId, CreateWeeklyLessonAssignmentRequest request);
        Task<GeneralResponse> UpdateWeeklyLessonRowAsync(Guid rowId, UpdateWeeklyLessonAssignmentRequest request);
        Task<GeneralResponse> DeleteWeeklyLessonRowAsync(Guid rowId);
    }
}
