using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.SystemConfiguration;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.SystemConfiguration;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/system-configuration-management")]
    [ApiController]
    public class SystemConfigurationManagementController : ControllerBase
    {
        private readonly ISystemConfigurationService _configService;

        public SystemConfigurationManagementController(ISystemConfigurationService configService)
        {
            _configService = configService;
        }

        [HttpPost("settings")]
        public async Task<ActionResult<ManageSystemSettingsResponse>> ManageSettings([FromBody] ManageSystemSettingsRequest request)
            => Ok(await _configService.ManageSystemSettingsAsync(request));

        [HttpPost("periods")]
        public async Task<ActionResult<ManageTimePeriodsResponse>> ManageTimePeriods([FromBody] ManageTimePeriodsRequest request)
            => Ok(await _configService.ManageTimePeriodsAsync(request));

        [HttpPost("timings")]
        public async Task<ActionResult<ConfigureTimingsResponse>> ConfigureTimings([FromBody] ConfigureTimingsRequest request)
            => Ok(await _configService.ConfigureTimingsAsync(request));

        [HttpGet("settings")]
        public async Task<ActionResult<SystemSettingsDto>> GetSystemSettings()
            => Ok(await _configService.GetSystemSettingsAsync(new GetSystemSettingsRequest()));

        [HttpGet("periods")]
        public async Task<ActionResult<PagedList<TimePeriodDto>>> GetAllTimePeriods([FromQuery] GetAllTimePeriodsRequest request)
            => Ok(await _configService.GetAllTimePeriodsAsync(request));

        [HttpGet("periods/{timePeriodId:guid}")]
        public async Task<ActionResult<TimePeriodDto>> GetTimePeriodById([FromRoute] Guid timePeriodId)
            => Ok(await _configService.GetTimePeriodByIdAsync(new GetTimePeriodByIdRequest { TimePeriodId = timePeriodId }));

        [HttpGet("security-settings")]
        public async Task<ActionResult<SecuritySettingsDto>> GetSecuritySettings()
            => Ok(await _configService.GetSecuritySettingsAsync(new GetSecuritySettingsRequest()));

        [HttpPut("settings/single")]
        public async Task<ActionResult<SystemSettingDto>> UpdateSystemSetting([FromBody] UpdateSystemSettingRequest request)
            => Ok(await _configService.UpdateSystemSettingAsync(request));

        [HttpPut("periods/{timePeriodId:guid}")]
        public async Task<ActionResult<TimePeriodDto>> UpdateTimePeriod([FromRoute] Guid timePeriodId, [FromBody] UpdateTimePeriodRequest request)
        {
            request.TimePeriodId = timePeriodId;
            return Ok(await _configService.UpdateTimePeriodAsync(request));
        }

        [HttpPost("logs")]
        public async Task<ActionResult<GetSystemLogsResponse>> GetLogs([FromBody] GetSystemLogsRequest request)
            => Ok(await _configService.GetSystemLogsAsync(request));

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

        [HttpPost("health")]
        public async Task<ActionResult<MonitorSystemHealthResponse>> MonitorHealth([FromBody] MonitorSystemHealthRequest request)
            => Ok(await _configService.MonitorSystemHealthAsync(request));

        [HttpGet("health")]
        public async Task<ActionResult<MonitorSystemHealthResponse>> MonitorHealthGet()
            => Ok(await _configService.MonitorSystemHealthAsync(new MonitorSystemHealthRequest()));

        [HttpPost("license")]
        public async Task<ActionResult<IssueUserLicenseResponse>> IssueLicense([FromBody] IssueUserLicenseRequest request)
            => Ok(await _configService.IssueUserLicensesAsync(request));
    }
}