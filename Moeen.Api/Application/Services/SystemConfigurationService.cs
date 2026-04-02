using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.SystemConfiguration;
using Moeen.Api.Shared.Responses.SystemConfiguration;

namespace Moeen.Api.Application.Services
{
    public class SystemConfigurationService : ISystemConfigurationService
    {
        public Task<ConfigureTimingsResponse> ConfigureTimingsAsync(ConfigureTimingsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GetSystemLogsResponse> GetSystemLogsAsync(GetSystemLogsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IssueUserLicenseResponse> IssueUserLicensesAsync(IssueUserLicenseRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ManageSystemSettingsResponse> ManageSystemSettingsAsync(ManageSystemSettingsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ManageTimePeriodsResponse> ManageTimePeriodsAsync(ManageTimePeriodsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MonitorSystemHealthResponse> MonitorSystemHealthAsync(MonitorSystemHealthRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
