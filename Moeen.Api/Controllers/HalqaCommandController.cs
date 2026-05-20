using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Halqa;
using Moeen.Shared.Responses;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HalqaCommandController : ControllerBase
    {
        private readonly IHalqaCommandService _HalqaCommandService;

        public HalqaCommandController(IHalqaCommandService HalqaCommandService)
        {
            _HalqaCommandService = HalqaCommandService;
        }

        /// <summary>
        /// أمر: إنشاء حلقة جديدة.
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<GeneralResponse>> CreateHalqa([FromBody] CreateHalqaRequest request)
        {
            try
            {
                var result = await _HalqaCommandService.CreateHalqaAsync(request);
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
        public async Task<ActionResult<GeneralResponse>> UpdateHalqa([FromBody] UpdateHalqaRequest request)
        {
            try
            {
                var result = await _HalqaCommandService.UpdateHalqaAsync(request);
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
        public async Task<ActionResult<GeneralResponse>> DeleteHalqa([FromBody] DeleteHalqaRequest request)
        {
            try
            {
                var deleted = await _HalqaCommandService.DeleteHalqaAsync(request);
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
        public async Task<ActionResult<GeneralResponse>> ReassignTeacher([FromBody] ReassignHalqaTeacherRequest request)
        {
            try
            {
                var result = await _HalqaCommandService.ReassignTeacherAsync(request);
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
        public async Task<ActionResult<GeneralResponse>> MoveToFouj([FromBody] MoveHalqaToFoujRequest request)
        {
            try
            {
                var result = await _HalqaCommandService.MoveToFoujAsync(request);
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