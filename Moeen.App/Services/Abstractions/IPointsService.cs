using Moeen.Shared.Responses.Points;

namespace Moeen.App.Services.Abstractions
{
    public interface IPointsService
    {
        Task<int> GetStudentPointsAsync(Guid studentId);
        Task<List<StudentPointsBreakdownDto>> GetStudentPointsBreakdownAsync(Guid studentId);
    }
}
