using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.CircleTeacherAssignment;
using Moeen.Shared.Requests.HalqaTeacherAssignment;
using Moeen.Shared.Requests.HalqaTeacherAssignment;
using Moeen.Shared.Responses;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HalqaTeacherAssignmentController : ControllerBase
    {
        private readonly IHalqaTeacherAssignmentService _assignmentService;

        public HalqaTeacherAssignmentController(IHalqaTeacherAssignmentService   assignmentService)
        {
            _assignmentService = assignmentService;
        }

        /// <summary>
        /// أمر: تعيين معلم مسؤول عن حلقة.
        /// </summary>
        [HttpPost("assign-teacher")]
        public async Task<ActionResult<GeneralResponse>> AssignTeacherToHalqa([FromBody] AssignTeacherToHalqaRequest request)
        {
            try
            {
                var result = await _assignmentService.AssignTeacherToHalqaAsync(request);
                if (!result.Success)
                    return BadRequest(GeneralResponse.BadRequest(result.Message));

                return Ok(GeneralResponse.Ok(result.Message, result));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء تعيين المعلم"));
            }
        }

        /// <summary>
        /// أمر: إزالة معلم من الإشراف على حلقة.
        /// </summary>
        [HttpPost("remove-teacher")]
        public async Task<ActionResult<GeneralResponse>> RemoveTeacherFromHalqa([FromBody] RemoveTeacherFromHalqaRequest request)
        {
            try
            {
                var result = await _assignmentService.RemoveTeacherFromHalqaAsync(request);
                if (!result.Success)
                    return BadRequest(GeneralResponse.BadRequest(result.Message));

                return Ok(GeneralResponse.Ok(result.Message, result));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء إزالة المعلم"));
            }
        }

        /// <summary>
        /// GET: جلب الحلقات المسندة لمعلم محدد.
        /// </summary>
        [HttpGet("teachers/{teacherId:guid}/halqas")]
        public async Task<ActionResult<GeneralResponse>> GetHalqasByTeacher (
            [FromRoute] Guid teacherId,
            [FromQuery] bool includeHistory = false)
        {
            try
            {
                var request = new GetHalqasByTeacherRequest { TeacherId = teacherId, IncludeHistory = includeHistory };
                var result = await _assignmentService.GetHalqasByTeacherAsync(request);
                return Ok(GeneralResponse.Ok("تم جلب الحلقات المسندة للمعلم بنجاح.", result));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب الحلقات"));
            }
        }

        /// <summary>
        /// GET: جلب المعلمين المسندين لحلقة محددة.
        /// </summary>
        [HttpGet("halqas/{halqaId:guid}/teachers")]
        public async Task<ActionResult<GeneralResponse>> GetTeachersByHalqa(
            [FromRoute] Guid halqaId,
            [FromQuery] bool includeInactive = false)
        {
            try
            {
                var request = new GetTeachersByHalqaRequest { HalqaId = halqaId, IncludeInactive = includeInactive };
                var result = await _assignmentService.GetTeachersByHalqaAsync(request);
                return Ok(GeneralResponse.Ok("تم جلب المعلمين المسندين للحلقة بنجاح.", result));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب المعلمين"));
            }
        }

        /// <summary>
        /// PUT: استبدال معلم بآخر في نفس الحلقة.
        /// </summary>
        [HttpPut("replace-teacher")]
        public async Task<ActionResult<GeneralResponse>> ReplaceTeacherInHalqa([FromBody] ReplaceTeacherRequest request)
        {
            try
            {
                var result = await _assignmentService.ReplaceTeacherInHalqaAsync(request);
                if (!result.Success)
                    return BadRequest(GeneralResponse.BadRequest(result.Message));

                return Ok(GeneralResponse.Ok(result.Message, result));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء استبدال المعلم"));
            }
        }
    }
}