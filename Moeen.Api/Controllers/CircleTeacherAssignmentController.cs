using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.CircleTeacherAssignment;
using Moeen.Shared.Responses;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircleTeacherAssignmentController : ControllerBase
    {
        private readonly ICircleTeacherAssignmentService _assignmentService;

        public CircleTeacherAssignmentController(ICircleTeacherAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        /// <summary>
        /// أمر: تعيين معلم مسؤول عن حلقة.
        /// </summary>
        [HttpPost("assign-teacher")]
        public async Task<ActionResult<GeneralResponse>> AssignTeacherToCircle([FromBody] AssignTeacherToCircleRequest request)
        {
            try
            {
                var result = await _assignmentService.AssignTeacherToCircleAsync(request);
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
        public async Task<ActionResult<GeneralResponse>> RemoveTeacherFromCircle([FromBody] RemoveTeacherFromCircleRequest request)
        {
            try
            {
                var result = await _assignmentService.RemoveTeacherFromCircleAsync(request);
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
        [HttpGet("teachers/{teacherId:guid}/circles")]
        public async Task<ActionResult<GeneralResponse>> GetCirclesByTeacher(
            [FromRoute] Guid teacherId,
            [FromQuery] bool includeHistory = false)
        {
            try
            {
                var request = new GetCirclesByTeacherRequest { TeacherId = teacherId, IncludeHistory = includeHistory };
                var result = await _assignmentService.GetCirclesByTeacherAsync(request);
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
        [HttpGet("circles/{circleId:guid}/teachers")]
        public async Task<ActionResult<GeneralResponse>> GetTeachersByCircle(
            [FromRoute] Guid circleId,
            [FromQuery] bool includeInactive = false)
        {
            try
            {
                var request = new GetTeachersByCircleRequest { CircleId = circleId, IncludeInactive = includeInactive };
                var result = await _assignmentService.GetTeachersByCircleAsync(request);
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
        public async Task<ActionResult<GeneralResponse>> ReplaceTeacherInCircle([FromBody] ReplaceTeacherRequest request)
        {
            try
            {
                var result = await _assignmentService.ReplaceTeacherInCircleAsync(request);
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