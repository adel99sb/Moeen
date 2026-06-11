using Moeen.Shared.Responses.DailyAssignments;

namespace Moeen.App.Services.Abstractions
{
    public interface IDailyAssignmentsService
    {
        Task<List<DailyAssignmentDto>> GetDailyAssignmentsAsync(Guid studentId, DateTime? date = null, Guid? halqaId = null);
    }
}
