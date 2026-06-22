using Moeen.Shared.Responses.Mobile;

namespace Moeen.App.Services.Abstractions
{
    public interface IStudentDashboardService
    {
        Task<StudentDashboardResponse> GetMyDashboardAsync();
    }
}
