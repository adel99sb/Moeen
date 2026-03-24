
using Moeen.Api.Shared.Requests.Identity;
using Moeen.Api.Shared.Responses.Identity;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IIdentityService
    {
        /// <summary>
        /// تسجيل الدخول للمستخدم
        /// </summary>
        /// <param name="request">بيانات تسجيل الدخول (ايميل وكلمة المرور)</param>
        /// <returns>نتيجة المصادقة مع التوكن وبيانات المستخدم</returns>
        Task<AuthResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// إنشاء حساب جديد
        /// </summary>
        /// <param name="request">بيانات التسجيل (الاسم، البريد، كلمة المرور، ...)</param>
        /// <returns>بيانات المستخدم المسجل</returns>
        Task<UserDto> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// تسجيل الخروج (قد يتطلب إبطال التوكن)
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// تحديث الملف الشخصي للمستخدم
        /// </summary>
        /// <param name="request">بيانات التحديث (معرف المستخدم والحقول المراد تعديلها)</param>
        /// <returns>بيانات المستخدم بعد التحديث</returns>
        Task<UserDto> UpdateProfileAsync(UpdateProfileRequest request);

        /// <summary>
        /// تغيير كلمة المرور
        /// </summary>
        /// <param name="request">بيانات تغيير كلمة المرور (معرف المستخدم، كلمة المرور الحالية والجديدة)</param>
        /// <returns>true إذا تم التغيير بنجاح، false إذا فشل</returns>
        Task<bool> ChangePasswordAsync(ChangePasswordRequest request);

        /// <summary>
        /// الحصول على بيانات المستخدم الحالي (بناءً على التوكن)
        /// </summary>
        /// <returns>بيانات المستخدم</returns>
        Task<UserDto> GetCurrentUserAsync();
    }
}