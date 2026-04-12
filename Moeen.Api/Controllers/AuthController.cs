using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Constants;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.Shared.Requests.Identity;
using Moeen.Api.Shared.Responses.Identity;
using System.Security.Claims;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // التحقق من عدم وجود مستخدم بنفس البريد
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest(new AuthResponse { IsSuccess = false, Message = "البريد الإلكتروني مستخدم بالفعل" });

            // إنشاء مستخدم جديد
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                name = request.Name,
                created_at = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new AuthResponse { IsSuccess = false, Message = errors });
            }

            // إضافة المستخدم لدور افتراضي (مثل "User")
            await _userManager.AddToRoleAsync(user, Roles.Student.ToString());
            return Ok("تم التسجيل بنجاح");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // البحث عن المستخدم بالبريد
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return NotFound(new AuthResponse { IsSuccess = false, Message = "البريد غير صحيحة" });

            // التحقق من كلمة المرور
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new AuthResponse { IsSuccess = false, Message = "كلمة المرور غير صحيحة" });

            // إنشاء توكن JWT
            var token = await _jwtService.GenerateTokenAsync(user);

            return Ok(new AuthResponse
            {
                IsSuccess = true,
                Message = "تم تسجيل الدخول بنجاح",
                Token = token
            });
        }

        /// <summary>
        /// GET (جديد): معلومات المستخدم الحالي من الـ claims.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public ActionResult<object> Me()
        {
            var id = User.FindFirst("ID")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            var name = User.FindFirst("Name")?.Value ?? User.Identity?.Name;

            return Ok(new { id, email, name });
        }

        /// <summary>
        /// GET (جديد): أدوار المستخدم الحالي.
        /// </summary>
        [Authorize]
        [HttpGet("me/roles")]
        public ActionResult<IEnumerable<string>> MyRoles()
        {
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
            return Ok(roles);
        }
    }
}