using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Identity;
using Moeen.Api.Shared.Responses.Identity;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]          // الرابط: api/identity
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        // حقن الخدمة عبر الـ Constructor
        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var result = await _identityService.LoginAsync(request);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterRequest request)
        {
            var result = await _identityService.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _identityService.LogoutAsync();
            return Ok();
        }

        [HttpPut("profile")]
        public async Task<ActionResult<UserDto>> UpdateProfile(UpdateProfileRequest request)
        {
            var result = await _identityService.UpdateProfileAsync(request);
            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<bool>> ChangePassword(ChangePasswordRequest request)
        {
            var result = await _identityService.ChangePasswordAsync(request);
            return Ok(result);
        }

        [HttpGet("current-user")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var result = await _identityService.GetCurrentUserAsync();
            return Ok(result);
        }
    }
}