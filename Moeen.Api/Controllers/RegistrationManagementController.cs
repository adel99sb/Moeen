using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Registration;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Registration;

namespace Moeen.Api.Controllers
{
    [Route("api/registration-management")]
    [ApiController]
    public class RegistrationManagementController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationManagementController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost("register-in-circle")]
        public async Task<ActionResult<OperationResponseDto>> RegisterInCircle([FromBody] RegisterInCircleRequest request)
            => Ok(await _registrationService.RegisterInCircleAsync(request));

        [HttpPost("unregister-from-circle")]
        public async Task<ActionResult<OperationResponseDto>> UnregisterFromCircle([FromBody] UnregisterFromCircleRequest request)
            => Ok(await _registrationService.UnregisterFromCircleAsync(request));

        [HttpPost("transfer-student")]
        public async Task<ActionResult<OperationResponseDto>> TransferStudent([FromBody] TransferStudentRequest request)
            => Ok(await _registrationService.TransferStudentAsync(request));

        /// <summary>
        /// GET:  ›«’Ì·  ”ÃÌ· »Ê«”ÿ… «·„⁄—›
        /// </summary>
        [HttpGet("registrations/{registrationId:guid}")]
        public async Task<ActionResult<RegistrationDto>> GetRegistrationById([FromRoute] Guid registrationId)
            => Ok(await _registrationService.GetRegistrationByIdAsync(new GetRegistrationByIdRequest { RegistrationId = registrationId }));

        /// <summary>
        /// GET: ÿ·«» «·Õ·ﬁ… «·„”Ã·Ì‰ „⁄ «· ’›Ì…
        /// </summary>
        [HttpGet("circles/{circleId:guid}/students")]
        public async Task<ActionResult<PagedList<CircleStudentDto>>> GetCircleStudents(
            [FromRoute] Guid circleId,
            [FromQuery] RegistrationStatus? status,
            [FromQuery] string? studentName,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
            => Ok(await _registrationService.GetCircleStudentsAsync(new GetCircleRegisteredStudentsRequest
            {
                CircleId = circleId,
                Status = status,
                StudentName = studentName,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));

        /// <summary>
        /// PUT:  ÕœÌÀ Õ«·…  ”ÃÌ·
        /// </summary>
        [HttpPut("registrations/{registrationId:guid}/status")]
        public async Task<ActionResult<RegistrationDto>> UpdateRegistrationStatus(
            [FromRoute] Guid registrationId,
            [FromBody] UpdateRegistrationStatusRequest request)
        {
            request.RegistrationId = registrationId;
            return Ok(await _registrationService.UpdateRegistrationStatusAsync(request));
        }
    }
}