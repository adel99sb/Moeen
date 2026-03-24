using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.SystemConfiguration;
using Moeen.Api.Shared.Responses.SystemConfiguration;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemConfigurationController : ControllerBase
    {
        private readonly ISystemConfigurationService _configService;

        public SystemConfigurationController(ISystemConfigurationService configService)
        {
            _configService = configService;
        }

        [HttpPost("settings")]
        public async Task<ActionResult<ManageSystemSettingsResponse>> ManageSettings(ManageSystemSettingsRequest request)
        {
            var result = await _configService.ManageSystemSettingsAsync(request);
            return Ok(result);
        }

        [HttpPost("periods")]
        public async Task<ActionResult<ManageTimePeriodsResponse>> ManageTimePeriods(ManageTimePeriodsRequest request)
        {
            var result = await _configService.ManageTimePeriodsAsync(request);
            return Ok(result);
        }

        [HttpPost("timings")]
        public async Task<ActionResult<ConfigureTimingsResponse>> ConfigureTimings(ConfigureTimingsRequest request)
        {
            var result = await _configService.ConfigureTimingsAsync(request);
            return Ok(result);
        }

        [HttpPost("logs")]
        public async Task<ActionResult<GetSystemLogsResponse>> GetLogs(GetSystemLogsRequest request)
        {
            var result = await _configService.GetSystemLogsAsync(request);
            return Ok(result);
        }

        [HttpPost("health")]
        public async Task<ActionResult<MonitorSystemHealthResponse>> MonitorHealth(MonitorSystemHealthRequest request)
        {
            var result = await _configService.MonitorSystemHealthAsync(request);
            return Ok(result);
        }

        [HttpPost("license")]
        public async Task<ActionResult<IssueUserLicenseResponse>> IssueLicense(IssueUserLicenseRequest request)
        {
            var result = await _configService.IssueUserLicensesAsync(request);
            return Ok(result);
        }
    }
}