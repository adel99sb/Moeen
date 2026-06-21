using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = nameof(Roles.Student))]
    [Route("api/mobile/student-profile")]
    public class MobileStudentProfileController : ControllerBase
    {
        private readonly IStudentMobileProfileService _profileService;
        private readonly ICurrentUserService _currentUserService;

        public MobileStudentProfileController(
            IStudentMobileProfileService profileService,
            ICurrentUserService currentUserService)
        {
            _profileService = profileService;
            _currentUserService = currentUserService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<GeneralResponse>> GetMyProfile()
        {
            var studentId = _currentUserService.CurrentUserId;
            if (!studentId.HasValue)
                return Unauthorized(GeneralResponse.Unauthorized("لم يتم التعرف على الطالب الحالي."));

            var response = await _profileService.GetProfileAsync(studentId.Value);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("me/note")]
        public async Task<ActionResult<GeneralResponse>> SubmitNote([FromBody] SubmitStudentProfileNoteRequest request)
        {
            var studentId = _currentUserService.CurrentUserId;
            if (!studentId.HasValue)
                return Unauthorized(GeneralResponse.Unauthorized("لم يتم التعرف على الطالب الحالي."));

            var response = await _profileService.SubmitNoteAsync(studentId.Value, request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
