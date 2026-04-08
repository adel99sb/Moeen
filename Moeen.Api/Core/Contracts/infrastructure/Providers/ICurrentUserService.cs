using Moeen.Api.Core.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.infrastructure.Providers
{
    public interface ICurrentUserService
    {
        /// <summary>معرف المستخدم الحالي (Guid)</summary>
        Guid? UserId { get; }

        /// <summary>اسم المستخدم الكامل</summary>
        string? UserName { get; }

        /// <summary>البريد الإلكتروني</summary>
        string? Email { get; }

        /// <summary>هل المستخدم مصادق عليه؟</summary>
        bool IsAuthenticated { get; }

        /// <summary>معرف الجامع الذي ينتمي إليه المستخدم (يؤخذ من الـ Claim)</summary>
        Guid? CurrentMosqueId { get; }

        /// <summary>قائمة أدوار المستخدم (أستاذ، طالب، مشرف، مدير)</summary>
        Task<List<string>> GetUserRolesAsync();

        /// <summary>قائمة الـ Claims الخاصة بالمستخدم</summary>
        Task<List<Claim>> GetUserClaimsAsync();

        /// <summary>التحقق من صلاحية محددة (من الـ Claims)</summary>
        Task<bool> HasPermissionAsync(string permission);

        /// <summary>جلب كيان المستخدم الكامل من قاعدة البيانات</summary>
        Task<User?> GetCurrentUserEntityAsync();
    }
}