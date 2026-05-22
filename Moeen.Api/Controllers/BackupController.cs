using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Backup;
using Moeen.Shared.Responses;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class BackupController : ControllerBase
    {
        private readonly IBackupService _backupService;

        public BackupController(IBackupService backupService)
        {
            _backupService = backupService;
        }

        /// <summary>
        /// ≈‰‘«¡ ‰”Œ… «Õ Ì«ÿÌ… ﬂ«„·…
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<GeneralResponse>> CreateBackup()
        {
            var response = await _backupService.CreateBackupAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Ã·» „⁄·Ê„«  ¬Œ— ‰”Œ… «Õ Ì«ÿÌ…
        /// </summary>
        [HttpGet("last-info")]
        public async Task<ActionResult<GeneralResponse>> GetLastBackupInfo()
        {
            var response = await _backupService.GetLastBackupInfoAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// «” ⁄«œ… ‰”Œ… «Õ Ì«ÿÌ…
        /// </summary>
        [HttpPost("restore")]
        public async Task<ActionResult<GeneralResponse>> RestoreBackup([FromBody] RestoreBackupRequest request)
        {
            var response = await _backupService.RestoreBackupAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}