using Moeen.Shared.Requests.LessonManagement;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.LessonManagement;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IWeeklyLessonService
    {
        Task<WeeklyLessonDashboardDto> GetTeacherWeeklyDashboardAsync(DateTime? date = null);
        Task<GeneralResponse> RecordLessonAttendanceAsync(RecordAttendanceRequest request);
        Task<List<LessonHistoryItemDto>> GetLessonHistoryAsync(Guid circleId, int pageNumber = 1, int pageSize = 20);

        Task<List<WeeklyLessonManagementDto>> GetWeeklyLessonsAsync();
        Task<GeneralResponse> CreateWeeklyLessonAsync(CreateWeeklyLessonRequest request);
        Task<GeneralResponse> UpdateWeeklyLessonAsync(Guid lessonId, UpdateWeeklyLessonRequest request);
        Task<GeneralResponse> DeleteWeeklyLessonAsync(Guid lessonId);
        Task<GeneralResponse> CreateWeeklyLessonRowAsync(Guid lessonId, CreateWeeklyLessonAssignmentRequest request);
        Task<GeneralResponse> UpdateWeeklyLessonRowAsync(Guid rowId, UpdateWeeklyLessonAssignmentRequest request);
        Task<GeneralResponse> DeleteWeeklyLessonRowAsync(Guid rowId);
    }
}
