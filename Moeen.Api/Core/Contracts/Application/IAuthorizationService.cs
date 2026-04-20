using Moeen.Api.Shared.Requests.Authorization;
using Moeen.Api.Shared.Responses.Analytics;
using Moeen.Api.Shared.Responses.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IAuthorizationService
    {
        /// <summary>
        /// التحقق من صلاحية وصول المستخدم لإذن معين
        /// </summary>
        Task<CheckAccessResponse> CheckAccessAsync(CheckAccessRequest request);

        /// <summary>
        /// إدارة الأدوار (إنشاء أو تحديث دور)
        /// </summary>
        Task<RoleDto> ManageRoleAsync(ManageRoleRequest request);

        /// <summary>
        /// تعيين دور لمستخدم
        /// </summary>
        Task<bool> AssignRoleToUserAsync(AssignRoleRequest request);

        /// <summary>
        /// إزالة دور من مستخدم
        /// </summary>
        Task<bool> RemoveRoleFromUserAsync(RemoveRoleRequest request);

        /// <summary>
        /// الحصول على قائمة أدوار المستخدم
        /// </summary>
        Task<UserRolesResponse> GetUserRolesAsync(UserRolesRequest request);

        /// <summary>
        /// الحصول على قائمة الأدوار مع الفلترة والترقيم
        /// </summary>
        Task<PagedResult<RoleDto>> GetAllRolesAsync(RoleFilter filter);

        /// <summary>
        /// الحصول على دور محدد بواسطة معرفه
        /// </summary>
        Task<RoleDto> GetRoleByIdAsync(Guid roleId);

        /// <summary>
        /// الحصول على قائمة جميع الصلاحيات المتاحة في النظام
        /// </summary>
        Task<List<PermissionDto>> GetAllPermissionsAsync();

        /// <summary>
        /// تحديث صلاحيات دور معين
        /// </summary>
        Task<RoleDto> UpdateRolePermissionsAsync(Guid roleId, UpdatePermissionsRequest request);

        /// <summary>
        /// حذف دور من النظام
        /// </summary>
        Task<bool> DeleteRoleAsync(Guid roleId);

        /// <summary>
        /// الحصول على قائمة صلاحيات مستخدم معين
        /// </summary>
        Task<List<PermissionDto>> GetUserPermissionsAsync(Guid userId);
    }
}