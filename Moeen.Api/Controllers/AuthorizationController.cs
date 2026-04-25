using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Authorization;
using Moeen.Api.Shared.Responses.Analytics;
using Moeen.Api.Shared.Responses.Authorization;
using System;
using System.Collections.Generic;

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
            => Ok(await _authorizationService.CheckAccessAsync(new CheckAccessRequest
            {
                UserId = userId,
                Permission = permission
            }));

        /// <summary>
        /// POST: إنشاء/تحديث دور.
        /// </summary>
        [HttpPost("manage-role")]
        public async Task<ActionResult<RoleDto>> ManageRole(ManageRoleRequest request)
            => Ok(await _authorizationService.ManageRoleAsync(request));

        /// <summary>
        /// POST: تعيين دور لمستخدم.
        /// </summary>
        [HttpPost("assign-role")]
        public async Task<ActionResult<bool>> AssignRoleToUser(AssignRoleRequest request)
            => Ok(await _authorizationService.AssignRoleToUserAsync(request));

        /// <summary>
        /// POST: إزالة دور من مستخدم.
        /// </summary>
        [HttpPost("remove-role")]
        public async Task<ActionResult<bool>> RemoveRoleFromUser(RemoveRoleRequest request)
            => Ok(await _authorizationService.RemoveRoleFromUserAsync(request));

        /// <summary>
        /// POST (قديم/متوافق): جلب أدوار المستخدم.
        /// </summary>
        [HttpPost("user-roles")]
        public async Task<ActionResult<UserRolesResponse>> GetUserRoles(UserRolesRequest request)
            => Ok(await _authorizationService.GetUserRolesAsync(request));

        /// <summary>
        /// GET (جديد): جلب أدوار المستخدم عبر Route.
        /// </summary>
        [HttpGet("users/{userId}/roles")]
        public async Task<ActionResult<UserRolesResponse>> GetUserRolesGet([FromRoute] string userId)
            => Ok(await _authorizationService.GetUserRolesAsync(new UserRolesRequest { UserId = userId }));

        /// <summary>
        /// GET: جلب كل الأدوار مع الفلترة والترقيم.
        /// </summary>
        [HttpGet("roles")]
        public async Task<ActionResult<PagedResult<RoleDto>>> GetAllRoles([FromQuery] RoleFilter filter)
            => Ok(await _authorizationService.GetAllRolesAsync(filter));

        /// <summary>
        /// GET: جلب دور محدد بواسطة المعرف.
        /// </summary>
        [HttpGet("roles/{roleId:guid}")]
        public async Task<ActionResult<RoleDto>> GetRoleById([FromRoute] Guid roleId)
            => Ok(await _authorizationService.GetRoleByIdAsync(roleId));

        /// <summary>
        /// GET: جلب جميع الصلاحيات المتاحة.
        /// </summary>
        [HttpGet("permissions")]
        public async Task<ActionResult<List<PermissionDto>>> GetAllPermissions()
            => Ok(await _authorizationService.GetAllPermissionsAsync());

        /// <summary>
        /// PUT: تحديث صلاحيات دور.
        /// </summary>
        [HttpPut("roles/{roleId:guid}/permissions")]
        public async Task<ActionResult<RoleDto>> UpdateRolePermissions([FromRoute] Guid roleId, [FromBody] UpdatePermissionsRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _authorizationService.UpdateRolePermissionsAsync(roleId, request));
        }

        /// <summary>
        /// DELETE: حذف دور.
        /// </summary>
        [HttpDelete("roles/{roleId:guid}")]
        public async Task<ActionResult<bool>> DeleteRole([FromRoute] Guid roleId)
            => Ok(await _authorizationService.DeleteRoleAsync(roleId));

        /// <summary>
        /// GET: جلب صلاحيات مستخدم بواسطة المعرف.
        /// </summary>
        [HttpGet("users/{userId:guid}/permissions")]
        public async Task<ActionResult<List<PermissionDto>>> GetUserPermissions([FromRoute] Guid userId)
            => Ok(await _authorizationService.GetUserPermissionsAsync(userId));
    }
}