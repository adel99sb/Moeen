using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = nameof(Roles.Student))]
    [Route("api/mobile/student-progress")]
    public class MobileStudentProgressController : ControllerBase
    {
        private readonly IStudentMobileProgressService _progressService;
        private readonly ICurrentUserService _currentUserService;

        public MobileStudentProgressController(
            IStudentMobileProgressService progressService,
            ICurrentUserService currentUserService)
        {
            _progressService = progressService;
            _currentUserService = currentUserService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<GeneralResponse>> GetMyProgress(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] ProgressRecordType? type)
        {
            var studentId = _currentUserService.CurrentUserId;
            if (!studentId.HasValue)
                return Unauthorized(GeneralResponse.Unauthorized("لم يتم التعرف على الطالب الحالي."));

            var response = await _progressService.GetProgressAsync(studentId.Value, from, to, type);
            return StatusCode(response.StatusCode, response);
        }
    }
}
