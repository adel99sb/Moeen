using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Shared.Requests.Verification;

namespace Moeen.Api.Controllers
{
    [ApiController]
    [Route("api/verification")]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationService _verificationService;

        public VerificationController(IVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        [HttpPost("send-code")]
        public async Task<IActionResult> SendCode([FromBody] SendCodeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var key = string.IsNullOrWhiteSpace(request.Key) ? request.Email : request.Key;

            await _verificationService.SendCodeToEmailAsync(request.Email, key);
            return Ok("Code sent");
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isValid = await _verificationService.VerifyCodeAsync(request.Key, request.Code);
            return isValid ? Ok("Verified") : BadRequest("Invalid code");
        }
    }
}