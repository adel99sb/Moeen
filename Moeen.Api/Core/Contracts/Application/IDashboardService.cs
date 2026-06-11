using Moeen.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IDashboardService
    {
        Task<GeneralResponse> GetAdminDashboardAsync();
        Task<GeneralResponse> GetMosqueDashboardAsync(Guid mosqueId);
        Task<GeneralResponse> GetTeacherDashboardAsync(Guid teacherId);
        Task<GeneralResponse> GetStudentDashboardAsync(Guid studentId);
    }
}