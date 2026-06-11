using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.User;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// إنشاء مستخدم جديد وتحديد نوعه مع إمكانية رفع صورة شخصية
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userService.RegisterAsync(registerRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while registering: {ex.Message}");
            }
        }

        /// <summary>
        /// تسجيل الدخول والحصول على الـ Access Token
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userService.LoginAsync(loginRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while logging in: {ex.Message}");
            }
        }

        /// <summary>
        /// طلب إرسال كود التحقق إلى البريد الإلكتروني (نسيت كلمة المرور / تأكيد الحساب)
        /// </summary>
        [AllowAnonymous]
        [HttpPost("send-verify-code")]
        public async Task<IActionResult> SendVerifyEmailCode([FromBody] SendVerifyEmailCodeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userService.SendVerifyEmailCodeAsync(request);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while sending verification code: {ex.Message}");
            }
        }

        /// <summary>
        /// التحقق من كود التأكيد المرسل للإيميل وتفعيل الحساب
        /// </summary>
        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userService.VerifyEmailAsync(request);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while verifying email: {ex.Message}");
            }
        }

        /// <summary>
        /// إعادة تعيين كلمة المرور بشكل مبسط عبر الـ Uid والكلمة الجديدة
        /// </summary>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userService.ResetPasswordAsync(request);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while resetting password: {ex.Message}");
            }
        }

        /// <summary>
        /// حذف حساب المستخدم نهائياً مع صورته الشخصية من السيرفر
        /// </summary>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting user: {ex.Message}");
            }
        }
        /// <summary>
        /// جلب قائمة بجميع المستخدمين المسجلين في النظام. GET api/user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await _userService.GetAllUsersAsync();
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving users: {ex.Message}");
            }
        }
        /// <summary>
        /// جلب قائمة بجميع المستخدمين المسجلين في النظام. GET api/user
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var result = await _userService.GetUserByIdAsync(userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving users: {ex.Message}");
            }
        }
    }
}