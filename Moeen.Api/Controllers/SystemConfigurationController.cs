using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.SystemConfiguration;
using Moeen.Api.Shared.Responses.SystemConfiguration;

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
            => Ok(await _configService.ManageSystemSettingsAsync(request));

        [HttpPost("periods")]
        public async Task<ActionResult<ManageTimePeriodsResponse>> ManageTimePeriods(ManageTimePeriodsRequest request)
            => Ok(await _configService.ManageTimePeriodsAsync(request));

        [HttpPost("timings")]
        public async Task<ActionResult<ConfigureTimingsResponse>> ConfigureTimings(ConfigureTimingsRequest request)
            => Ok(await _configService.ConfigureTimingsAsync(request));

        /// <summary>
        /// POST (قديم/متوافق): جلب السجلات.
        /// </summary>
        [HttpPost("logs")]
        public async Task<ActionResult<GetSystemLogsResponse>> GetLogs(GetSystemLogsRequest request)
            => Ok(await _configService.GetSystemLogsAsync(request));

        /// <summary>
        /// GET (جديد): جلب السجلات عبر Query.
        /// </summary>
        [HttpGet("logs")]
        public async Task<ActionResult<GetSystemLogsResponse>> GetLogsGet(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? level = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
            => Ok(await _configService.GetSystemLogsAsync(new GetSystemLogsRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Level = level ?? string.Empty,
                From = from,
                To = to
            }));

        /// <summary>
        /// POST (قديم/متوافق): فحص صحة النظام.
        /// </summary>
        [HttpPost("health")]
        public async Task<ActionResult<MonitorSystemHealthResponse>> MonitorHealth(MonitorSystemHealthRequest request)
            => Ok(await _configService.MonitorSystemHealthAsync(request));

        /// <summary>
        /// GET (جديد): فحص صحة النظام مباشرة.
        /// </summary>
        [HttpGet("health")]
        public async Task<ActionResult<MonitorSystemHealthResponse>> MonitorHealthGet()
            => Ok(await _configService.MonitorSystemHealthAsync(new MonitorSystemHealthRequest()));

        [HttpPost("license")]
        public async Task<ActionResult<IssueUserLicenseResponse>> IssueLicense(IssueUserLicenseRequest request)
            => Ok(await _configService.IssueUserLicensesAsync(request));
    }
}