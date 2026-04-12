using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Registration;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Shared.Requests.Verification;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly IVerificationService _verificationService;

        public RegistrationController(IRegistrationService registrationService, IVerificationService verificationService)
        {
            _registrationService = registrationService;
            _verificationService = verificationService;
        }

        /// <summary>
        /// POST (قديم/متوافق): تسجيل طالب في حلقة.
        /// </summary>
        [HttpPost("register-in-circle")]
        public async Task<ActionResult<OperationResponse>> RegisterInCircle(RegisterInCircleRequest request)
            => Ok(await _registrationService.RegisterInCircleAsync(request));

        /// <summary>
        /// POST (جديد - REST Alias): تسجيل طالب في حلقة عبر Route.
        /// </summary>
        [HttpPost("circles/{circleId:guid}/students/{studentId:guid}")]
        public async Task<ActionResult<OperationResponse>> RegisterInCircleByRoute([FromRoute] Guid circleId, [FromRoute] Guid studentId)
            => Ok(await _registrationService.RegisterInCircleAsync(new RegisterInCircleRequest
            {
                CircleId = circleId,
                StudentId = studentId
            }));

        /// <summary>
        /// POST (قديم/متوافق): إلغاء تسجيل طالب من حلقة.
        /// </summary>
        [HttpPost("unregister-from-circle")]
        public async Task<ActionResult<OperationResponse>> UnregisterFromCircle(UnregisterFromCircleRequest request)
            => Ok(await _registrationService.UnregisterFromCircleAsync(request));

        /// <summary>
        /// DELETE (جديد - REST): إلغاء تسجيل طالب من حلقة عبر Route.
        /// </summary>
        [HttpDelete("circles/{circleId:guid}/students/{studentId:guid}")]
        public async Task<ActionResult<OperationResponse>> UnregisterFromCircleByRoute([FromRoute] Guid circleId, [FromRoute] Guid studentId)
            => Ok(await _registrationService.UnregisterFromCircleAsync(new UnregisterFromCircleRequest
            {
                CircleId = circleId,
                StudentId = studentId
            }));

        /// <summary>
        /// أمر: نقل طالب بين حلقتين.
        /// </summary>
        [HttpPost("transfer-student")]
        public async Task<ActionResult<OperationResponse>> TransferStudent(TransferStudentRequest request)
                => Ok(await _registrationService.TransferStudentAsync(request));

        /// <summary>
        /// POST (قديم/متوافق): إرسال كود التحقق.
        /// </summary>
        [HttpPost("send-code")]
        public async Task<IActionResult> SendCode([FromBody] SendCodeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var key = string.IsNullOrWhiteSpace(request.Key) ? request.Email : request.Key;

            await _verificationService.SendCodeToEmailAsync(request.Email, key);
            return Ok("Code sent");
        }

        /// <summary>
        /// POST (قديم/متوافق): التحقق من الكود.
        /// </summary>
        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isValid = await _verificationService.VerifyCodeAsync(request.Key, request.Code);
            return isValid ? Ok("Verified") : BadRequest("Invalid code");
        }

        /// <summary>
        /// GET (جديد): التحقق من الكود عبر Query.
        /// </summary>
        [HttpGet("verify-code")]
        public async Task<IActionResult> VerifyCodeGet([FromQuery] string key, [FromQuery] string code)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(code))
                return BadRequest("Key and code are required.");

            var isValid = await _verificationService.VerifyCodeAsync(key, code);
            return isValid ? Ok("Verified") : BadRequest("Invalid code");
        }
    }
}