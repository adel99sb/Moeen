using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementations
{
    public class BackupService : IBackupService
    {
        private readonly BackupApiClient _client;

        public BackupService(BackupApiClient client)
        {
            _client = client;
        }

        public Task<GeneralResponse> CreateBackupAsync()
            => _client.CreateBackupAsync();

        public Task<GeneralResponse> GetLastBackupInfoAsync()
            => _client.GetLastBackupInfoAsync();
    }
}
