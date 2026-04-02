using Moeen.Api.Shared.Requests.Authorization;
using Moeen.Api.Shared.Responses.Authorization;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IAuthorizationService
    {
        /// <summary>
        /// التحقق من صلاحية وصول المستخدم لإذن معين
        /// </summary>
        /// <param name="request">معرف المستخدم والإذن</param>
        /// <returns>نتيجة التحقق</returns>
        Task<CheckAccessResponse> CheckAccessAsync(CheckAccessRequest request);

        /// <summary>
        /// إدارة الأدوار (إنشاء أو تحديث دور)
        /// </summary>
        /// <param name="request">بيانات الدور (إذا كان RoleId موجوداً فهذا تحديث، وإلا إنشاء)</param>
        /// <returns>الدور بعد الإنشاء/التحديث</returns>
        Task<Shared.Requests.Authorization.RoleDto> ManageRoleAsync(ManageRoleRequest request);

        /// <summary>
        /// تعيين دور لمستخدم
        /// </summary>
        /// <param name="request">معرف المستخدم ومعرف الدور</param>
        /// <returns>true إذا تم التعيين بنجاح</returns>
        Task<bool> AssignRoleToUserAsync(AssignRoleRequest request);

        /// <summary>
        /// إزالة دور من مستخدم
        /// </summary>
        /// <param name="request">معرف المستخدم ومعرف الدور</param>
        /// <returns>true إذا تمت الإزالة بنجاح</returns>
        Task<bool> RemoveRoleFromUserAsync(RemoveRoleRequest request);

        /// <summary>
        /// الحصول على قائمة أدوار المستخدم
        /// </summary>
        /// <param name="request">معرف المستخدم</param>
        /// <returns>قائمة الأدوار</returns>
        Task<UserRolesResponse> GetUserRolesAsync(UserRolesRequest request);
    }
}