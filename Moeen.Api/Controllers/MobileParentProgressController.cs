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
    [Authorize(Roles = nameof(Roles.ParentSudent))]
    [Route("api/mobile/parent-progress")]
    public class MobileParentProgressController : ControllerBase
    {
        private readonly IParentMobileProgressService _progressService;
        private readonly ICurrentUserService _currentUserService;

        public MobileParentProgressController(
            IParentMobileProgressService progressService,
            ICurrentUserService currentUserService)
        {
            _progressService = progressService;
            _currentUserService = currentUserService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<GeneralResponse>> GetMyChildProgress(
            [FromQuery] Guid? childId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] ProgressRecordType? type)
        {
            var parentId = _currentUserService.CurrentUserId;
            if (!parentId.HasValue)
                return Unauthorized(GeneralResponse.Unauthorized("لم يتم التعرف على ولي الأمر الحالي."));

            var response = await _progressService.GetProgressAsync(parentId.Value, childId, from, to, type);
            return StatusCode(response.StatusCode, response);
        }
    }
}
