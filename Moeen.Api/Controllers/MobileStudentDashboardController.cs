using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;

namespace Moeen.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = nameof(Roles.Student))]
    [Route("api/mobile/student-dashboard")]
    public class MobileStudentDashboardController : ControllerBase
    {
        private readonly IStudentMobileDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUserService;

        public MobileStudentDashboardController(
            IStudentMobileDashboardService dashboardService,
            ICurrentUserService currentUserService)
        {
            _dashboardService = dashboardService;
            _currentUserService = currentUserService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<GeneralResponse>> GetMyDashboard()
        {
            var studentId = _currentUserService.CurrentUserId;
            if (!studentId.HasValue)
                return Unauthorized(GeneralResponse.Unauthorized("لم يتم التعرف على الطالب الحالي."));

            var response = await _dashboardService.GetDashboardAsync(studentId.Value);
            return StatusCode(response.StatusCode, response);
        }
    }
}
