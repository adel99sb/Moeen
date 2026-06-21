using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = nameof(Roles.ParentSudent))]
    [Route("api/mobile/parent-profile")]
    public class MobileParentProfileController : ControllerBase
    {
        private readonly IParentMobileProfileService _profileService;
        private readonly ICurrentUserService _currentUserService;

        public MobileParentProfileController(
            IParentMobileProfileService profileService,
            ICurrentUserService currentUserService)
        {
            _profileService = profileService;
            _currentUserService = currentUserService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<GeneralResponse>> GetMyProfile()
        {
            var parentId = _currentUserService.CurrentUserId;
            if (!parentId.HasValue)
                return Unauthorized(GeneralResponse.Unauthorized("لم يتم التعرف على ولي الأمر الحالي."));

            var response = await _profileService.GetProfileAsync(parentId.Value);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("me/note")]
        public async Task<ActionResult<GeneralResponse>> SubmitNote([FromBody] SubmitParentProfileNoteRequest request)
        {
            var parentId = _currentUserService.CurrentUserId;
            if (!parentId.HasValue)
                return Unauthorized(GeneralResponse.Unauthorized("لم يتم التعرف على ولي الأمر الحالي."));

            var response = await _profileService.SubmitNoteAsync(parentId.Value, request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
