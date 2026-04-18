using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Authorization;
using Moeen.Api.Shared.Responses.Authorization;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// POST (قديم/متوافق): التحقق من صلاحية وصول المستخدم.
        /// </summary>
        [HttpPost("check-access")]
        public async Task<ActionResult<CheckAccessResponse>> CheckAccess(CheckAccessRequest request)
            => Ok(await _authorizationService.CheckAccessAsync(request));

        /// <summary>
        /// GET (جديد): التحقق من الصلاحية عبر Query.
        /// </summary>
        [HttpGet("check-access")]
        public async Task<ActionResult<CheckAccessResponse>> CheckAccessGet([FromQuery] string userId, [FromQuery] string permission)
            => Ok(await _authorizationService.CheckAccessAsync(new CheckAccessRequest { UserId = userId, Permission = permission }));

        [HttpPost("manage-role")]
        public async Task<ActionResult<RoleDto>> ManageRole(ManageRoleRequest request)
            => Ok(await _authorizationService.ManageRoleAsync(request));

        [HttpPost("assign-role")]
        public async Task<ActionResult<bool>> AssignRoleToUser(AssignRoleRequest request)
            => Ok(await _authorizationService.AssignRoleToUserAsync(request));

        [HttpPost("remove-role")]
        public async Task<ActionResult<bool>> RemoveRoleFromUser(RemoveRoleRequest request)
            => Ok(await _authorizationService.RemoveRoleFromUserAsync(request));

        /// <summary>
        /// POST (قديم/متوافق): أدوار المستخدم.
        /// </summary>
        [HttpPost("user-roles")]
        public async Task<ActionResult<UserRolesResponse>> GetUserRoles(UserRolesRequest request)
            => Ok(await _authorizationService.GetUserRolesAsync(request));

        /// <summary>
        /// GET (جديد): أدوار المستخدم عبر Route.
        /// </summary>
        [HttpGet("users/{userId}/roles")]
        public async Task<ActionResult<UserRolesResponse>> GetUserRolesGet([FromRoute] string userId)
            => Ok(await _authorizationService.GetUserRolesAsync(new UserRolesRequest { UserId = userId }));
    }
}