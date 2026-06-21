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
    [Route("api/mobile/parent-dashboard")]
    public class MobileParentDashboardController : ControllerBase
    {
        private readonly IParentMobileDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUserService;

        public MobileParentDashboardController(
            IParentMobileDashboardService dashboardService,
            ICurrentUserService currentUserService)
        {
            _dashboardService = dashboardService;
            _currentUserService = currentUserService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<GeneralResponse>> GetMyDashboard([FromQuery] Guid? childId)
        {
            var parentId = _currentUserService.CurrentUserId;
            if (!parentId.HasValue)
                return Unauthorized(GeneralResponse.Unauthorized("لم يتم التعرف على ولي الأمر الحالي."));

            var response = await _dashboardService.GetDashboardAsync(parentId.Value, childId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
