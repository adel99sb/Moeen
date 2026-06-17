using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IBackupService
    {
        Task<GeneralResponse> CreateBackupAsync();
        Task<GeneralResponse> GetLastBackupInfoAsync();
    }
}
