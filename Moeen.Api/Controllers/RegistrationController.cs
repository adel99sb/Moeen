using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Registration;
using Moeen.Api.Shared.Responses.Registration;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        /// <summary>
        /// تسجيل طالب في حلقة دراسية
        /// </summary>
        [HttpPost("register-in-circle")]
        public async Task<ActionResult<OperationResponse>> RegisterInCircle(RegisterInCircleRequest request)
        {
            var result = await _registrationService.RegisterInCircleAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// إلغاء تسجيل طالب من حلقة
        /// </summary>
        [HttpPost("unregister-from-circle")]
        public async Task<ActionResult<OperationResponse>> UnregisterFromCircle(UnregisterFromCircleRequest request)
        {
            var result = await _registrationService.UnregisterFromCircleAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// نقل طالب بين حلقتين
        /// </summary>
        [HttpPost("transfer-student")]
        public async Task<ActionResult<OperationResponse>> TransferStudent(TransferStudentRequest request)
        {
            var result = await _registrationService.TransferStudentAsync(request);
            return Ok(result);
        }
    }
}