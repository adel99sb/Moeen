using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Registration;
using Moeen.Shared.Responses;
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

        [HttpPost("register")]
        public async Task<ActionResult<GeneralResponse>> RegisterInCircle([FromBody] RegisterInCircleRequest request)
            => Ok(await _registrationService.RegisterInCircleAsync(request));

        [HttpPost("unregister")]
        public async Task<ActionResult<GeneralResponse>> UnregisterFromCircle([FromBody] UnregisterFromCircleRequest request)
            => Ok(await _registrationService.UnregisterFromCircleAsync(request));

        [HttpPost("transfer")]
        public async Task<ActionResult<GeneralResponse>> TransferStudent([FromBody] TransferStudentRequest request)
            => Ok(await _registrationService.TransferStudentAsync(request));

        [HttpGet("students")]
        public async Task<ActionResult<GeneralResponse>> GetCircleStudents([FromQuery] GetCircleRegisteredStudentsRequest request)
            => Ok(await _registrationService.GetCircleStudentsAsync(request));
    }
}