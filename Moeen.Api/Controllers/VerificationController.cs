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

        /// <summary>
        /// POST (ﬁœÌ„/„ Ê«›ﬁ): ≈—”«· ﬂÊœ «· Õﬁﬁ.
        /// </summary>
        [HttpPost("send-code")]
        public async Task<IActionResult> SendCode([FromBody] SendCodeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var key = string.IsNullOrWhiteSpace(request.Key) ? request.Email : request.Key;
            //await _verificationService.SendCodeToEmailAsync(request.Email, key);

            return Ok("Code sent");
        }

        /// <summary>
        /// POST (ﬁœÌ„/„ Ê«›ﬁ): «· Õﬁﬁ „‰ «·ﬂÊœ ⁄»— Body.
        /// </summary>
        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //var isValid = await _verificationService.VerifyCodeAsync(request.Key, request.Code);
            //return isValid ? Ok("Verified") : BadRequest("Invalid code");
            return BadRequest("Invalid code");
        }

        /// <summary>
        /// GET (ÃœÌœ): «· Õﬁﬁ „‰ «·ﬂÊœ ⁄»— Query.
        /// </summary>
        [HttpGet("verify-code")]
        public async Task<IActionResult> VerifyCodeGet([FromQuery] string key, [FromQuery] string code)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(code))
                return BadRequest("Key and code are required.");

            //var isValid = await _verificationService.VerifyCodeAsync(key, code);
            //return isValid ? Ok("Verified") : BadRequest("Invalid code");
            return BadRequest("Invalid code");
        }
    }
}