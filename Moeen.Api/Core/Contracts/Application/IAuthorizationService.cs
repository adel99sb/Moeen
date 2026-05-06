using Moeen.Shared.Requests.Authorization;
using Moeen.Shared.Responses;
using System;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IAuthorizationService
    {
        // ========================= Account Management =========================

        // جلب حسابات المستخدمين حسب النوع مع البحث والتصفية
        Task<GeneralResponse> GetAccountsAsync(AccountFilterRequest request);

        // جلب تفاصيل حساب محدد للتعديل
        Task<GeneralResponse> GetAccountByIdAsync(Guid userId);

        // إنشاء أو تحديث حساب مستخدم
        Task<GeneralResponse> UpsertAccountAsync(UpsertAccountRequest request);

        // تغيير حالة الحساب (نشط/متوقف)
        Task<GeneralResponse> UpdateAccountStatusAsync(UpdateAccountStatusRequest request);

        // حذف حساب مستخدم
        Task<GeneralResponse> DeleteAccountAsync(Guid userId);

        // ========================= Authorization / Roles =========================

        // التحقق من صلاحية الوصول لمورد أو إجراء معين
        Task<GeneralResponse> CheckAccessAsync(CheckAccessRequest request);

        // إدارة الأدوار: إنشاء، تحديث، أو حذف دور في النظام
        Task<GeneralResponse> ManageRoleAsync(ManageRoleRequest request);

        // تعيين دور لمستخدم معين لمنحه الصلاحيات المرتبطة بهذا الدور
        Task<GeneralResponse> AssignRoleToUserAsync(AssignRoleRequest request);

        // إزالة دور من مستخدم معين لسحب الصلاحيات المرتبطة بهذا الدور
        Task<GeneralResponse> RemoveRoleFromUserAsync(RemoveRoleRequest request);

        // جلب جميع الأدوار الممنوحة لمستخدم معين
        Task<GeneralResponse> GetUserRolesAsync(UserRolesRequest request);

        // جلب جميع الأدوار المتاحة في النظام مع دعم البحث والتصفية
        Task<GeneralResponse> GetAllRolesAsync(RoleFilter filter);

        // جلب تفاصيل دور محدد بناءً على معرفه
        Task<GeneralResponse> GetRoleByIdAsync(Guid roleId);

        // جلب جميع الصلاحيات المتاحة في النظام لتستخدم في تعيينها للأدوار
        Task<GeneralResponse> GetAllPermissionsAsync();

        // تحديث الصلاحيات الممنوحة لدور معين (إضافة أو إزالة صلاحيات)
        Task<GeneralResponse> UpdateRolePermissionsAsync(Guid roleId, UpdatePermissionsRequest request);

        // حذف دور من النظام (مع التحقق من عدم استخدامه من قبل المستخدمين)
        Task<GeneralResponse> DeleteRoleAsync(Guid roleId);

        // جلب جميع الصلاحيات الفعالة لمستخدم معين (مباشرة أو عبر الأدوار)
        Task<GeneralResponse> GetUserPermissionsAsync(Guid userId);
    }
}