using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Circle;
using Moeen.Shared.Responses;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircleCommandController : ControllerBase
    {
        private readonly ICircleCommandService _circleCommandService;

        public CircleCommandController(ICircleCommandService circleCommandService)
        {
            _circleCommandService = circleCommandService;
        }

        /// <summary>
        /// أمر: إنشاء حلقة جديدة.
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<GeneralResponse>> CreateCircle([FromBody] CreateCircleRequest request)
        {
            try
            {
                var result = await _circleCommandService.CreateCircleAsync(request);
                return Ok(GeneralResponse.Ok("تم إنشاء الحلقة بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, GeneralResponse.InternalError($"فشل الإنشاء: {ex.Message}"));
            }
        }

        /// <summary>
        /// أمر: تحديث بيانات حلقة.
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<GeneralResponse>> UpdateCircle([FromBody] UpdateCircleRequest request)
        {
            try
            {
                var result = await _circleCommandService.UpdateCircleAsync(request);
                return Ok(GeneralResponse.Ok("تم تحديث الحلقة بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, GeneralResponse.InternalError($"فشل التحديث: {ex.Message}"));
            }
        }

        /// <summary>
        /// أمر: حذف حلقة.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<ActionResult<GeneralResponse>> DeleteCircle([FromBody] DeleteCircleRequest request)
        {
            try
            {
                var deleted = await _circleCommandService.DeleteCircleAsync(request);
                if (!deleted)
                    return NotFound(GeneralResponse.NotFound("لم يتم العثور على الحلقة."));

                return Ok(GeneralResponse.Ok("تم حذف الحلقة بنجاح.", deleted));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, GeneralResponse.InternalError($"فشل الحذف: {ex.Message}"));
            }
        }

        /// <summary>
        /// أمر: إعادة تعيين معلم للحلقة.
        /// </summary>
        [HttpPut("reassign-teacher")]
        public async Task<ActionResult<GeneralResponse>> ReassignTeacher([FromBody] ReassignCircleTeacherRequest request)
        {
            try
            {
                var result = await _circleCommandService.ReassignTeacherAsync(request);
                return Ok(GeneralResponse.Ok("تمت إعادة تعيين المعلم بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, GeneralResponse.InternalError($"فشل الإعادة: {ex.Message}"));
            }
        }

        /// <summary>
        /// أمر: نقل الحلقة إلى فوج آخر.
        /// </summary>
        [HttpPut("move-to-fouj")]
        public async Task<ActionResult<GeneralResponse>> MoveToFouj([FromBody] MoveCircleToFoujRequest request)
        {
            try
            {
                var result = await _circleCommandService.MoveToFoujAsync(request);
                return Ok(GeneralResponse.Ok("تم نقل الحلقة بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, GeneralResponse.InternalError($"فشل النقل: {ex.Message}"));
            }
        }
    }
}