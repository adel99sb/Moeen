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
        /// التحقق من صلاحية وصول المستخدم لإذن معين
        /// </summary>
        [HttpPost("check-access")]
        public async Task<ActionResult<CheckAccessResponse>> CheckAccess(CheckAccessRequest request)
        {
            var result = await _authorizationService.CheckAccessAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// إدارة الأدوار (إنشاء أو تحديث دور)
        /// </summary>
        [HttpPost("manage-role")]
        public async Task<ActionResult<RoleDto>> ManageRole(ManageRoleRequest request)
        {
            var result = await _authorizationService.ManageRoleAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تعيين دور لمستخدم
        /// </summary>
        [HttpPost("assign-role")]
        public async Task<ActionResult<bool>> AssignRoleToUser(AssignRoleRequest request)
        {
            var result = await _authorizationService.AssignRoleToUserAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// إزالة دور من مستخدم
        /// </summary>
        [HttpPost("remove-role")]
        public async Task<ActionResult<bool>> RemoveRoleFromUser(RemoveRoleRequest request)
        {
            var result = await _authorizationService.RemoveRoleFromUserAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على قائمة أدوار المستخدم
        /// </summary>
        [HttpPost("user-roles")]
        public async Task<ActionResult<UserRolesResponse>> GetUserRoles(UserRolesRequest request)
        {
            var result = await _authorizationService.GetUserRolesAsync(request);
            return Ok(result);
        }
    }
}