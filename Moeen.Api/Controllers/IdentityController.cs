using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Identity;
using Moeen.Api.Shared.Responses.Identity;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var result = await _identityService.LoginAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("register")]  
        public async Task<ActionResult<UserDto>> Register(RegisterRequest request)
            => Ok(await _identityService.RegisterAsync(request));

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _identityService.LogoutAsync();
            return Ok();
        }

        [HttpPut("profile")]
        public async Task<ActionResult<UserDto>> UpdateProfile(UpdateProfileRequest request)
            => Ok(await _identityService.UpdateProfileAsync(request));

        [HttpPost("change-password")]
        public async Task<ActionResult<bool>> ChangePassword(ChangePasswordRequest request)
            => Ok(await _identityService.ChangePasswordAsync(request));

        /// <summary>
        /// GET (أساسي): بيانات المستخدم الحالي.
        /// </summary>
        [HttpGet("current-user")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
            => Ok(await _identityService.GetCurrentUserAsync());

        /// <summary>
        /// GET (جديد - Alias): نفس current-user لكن endpoint أقصر.
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> Me()
            => Ok(await _identityService.GetCurrentUserAsync());
    }
}