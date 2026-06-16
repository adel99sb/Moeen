using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.Identity;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {   
        private readonly IUserService _userService;

        private Dictionary<string, string[]> GetModelStateErrors()
        {
            return ModelState
                .Where(item => item.Value?.Errors.Count > 0)
                .ToDictionary(
                    item => item.Key,
                    item => item.Value!.Errors
                        .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Invalid value." : error.ErrorMessage)
                        .ToArray());
        }

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Register (moved here). POST api/user/register
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
                return Moeen.Shared.Responses.GeneralResponse.BadRequest("Invalid request data.", GetModelStateErrors()).ToActionResult();

            try
            {
                var result = await _userService.RegisterAsync(registerRequest);
                return result.ToActionResult();
            }
            catch (Exception)
            {
                return Moeen.Shared.Responses.GeneralResponse.InternalError("An error occurred while registering.").ToActionResult();
            }
        }

        /// <summary>
        /// Login (moved here). POST api/user/login
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
                return Moeen.Shared.Responses.GeneralResponse.BadRequest("Invalid request data.", GetModelStateErrors()).ToActionResult();

            try
            {
                var result = await _userService.LoginAsync(loginRequest);
                return result.ToActionResult();
            }
            catch (Exception)
            {
                return Moeen.Shared.Responses.GeneralResponse.InternalError("An error occurred while logging in.").ToActionResult();
            }
        }

        /// <summary>
        /// جلب مستخدمين مع تصفح (body: PaginationRequest, optional query: keyword)
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> GetAllUsers([FromBody] PaginationRequest paginationRequest, [FromQuery] string? keyword = null)
        {
            if (!ModelState.IsValid)
                return Moeen.Shared.Responses.GeneralResponse.BadRequest("Invalid request data.", GetModelStateErrors()).ToActionResult();

            var result = await _userService.GetAllUsersAsync(paginationRequest, keyword);
            return result.ToActionResult();
        }

        /// <summary>
        /// جلب بيانات مستخدم بواسطة المعرف
        /// </summary>
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserById([FromRoute] Guid userId)
        {
            var result = await _userService.GetUserByIdAsync(userId);
            return result.ToActionResult();
        }

        /// <summary>
        /// تعديل إيميل المستخدم الحالي (body: string email)
        /// </summary>
        [HttpPut("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var result = await _userService.ChangeUserEmailAsync(email);
            return result.ToActionResult();
        }

        /// <summary>
        /// إرسال رابط إعادة كلمة السر إلى الإيميل (يسمح للمجهولين)
        /// </summary>
        [AllowAnonymous]
        [HttpPost("send-reset-url")]
        public async Task<IActionResult> SendPasswordResetUrl([FromBody] SendPasswordResetUrlRequest request)
        {
            var result = await _userService.SendPasswordResetUrlAsync(request);
            return result.ToActionResult();
        }

        /// <summary>
        /// إعادة تعيين كلمة السر (body: ChangePasswordRequest) — يجب تزويد Token في الطلب
        /// </summary>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return Moeen.Shared.Responses.GeneralResponse.BadRequest("Invalid request data.", GetModelStateErrors()).ToActionResult();

            var result = await _userService.ResetPasswordAsync(request);
            return result.ToActionResult();
        }

        /// <summary>
        /// إرسال كود تحقق للإيميل (يمكن أن يستقبل Email داخل الطلب أو يستخدم المستخدم الحالي)
        /// </summary>
        [HttpPost("send-verify-code")]
        public async Task<IActionResult> SendVerifyEmailCode([FromBody] SendVerifyEmailCodeRequest request)
        {
            var result = await _userService.SendVerifyEmailCodeAsync(request);
            return result.ToActionResult();
        }

        /// <summary>
        /// تحقق من كود التفعيل (body: VerifyEmailRequest)
        /// </summary>
        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var result = await _userService.VerifyEmailAsync(request);
            return result.ToActionResult();
        }
    }
}

