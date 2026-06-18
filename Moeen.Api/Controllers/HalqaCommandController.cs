using System;
using System.Threading.Tasks;
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
        private readonly IHalqaCommandService _halqaCommandService;

        public HalqaCommandController(IHalqaCommandService halqaCommandService)
        {
            _halqaCommandService = halqaCommandService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<GeneralResponse>> CreateHalqa([FromBody] CreateHalqaRequest request)
        {
            try
            {
                var result = await _halqaCommandService.CreateHalqaAsync(request);
                if (!result.Success)
                    return StatusCode(result.StatusCode, result);

                return Ok(result);
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

        [HttpPut("update")]
        public async Task<ActionResult<GeneralResponse>> UpdateHalqa([FromBody] UpdateHalqaRequest request)
        {
            try
            {
                var result = await _halqaCommandService.UpdateHalqaAsync(request);
                if (!result.Success)
                    return StatusCode(result.StatusCode, result);

                return Ok(result);
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

        [HttpDelete("delete")]
        public async Task<ActionResult<GeneralResponse>> DeleteHalqa([FromBody] DeleteHalqaRequest request)
        {
            var deleted = await _halqaCommandService.DeleteHalqaAsync(request);

            if (!deleted.Success)
                return StatusCode(deleted.StatusCode, deleted);

            return Ok(deleted);
        }

        [HttpPut("reassign-teacher")]
        public async Task<ActionResult<GeneralResponse>> ReassignTeacher([FromBody] ReassignHalqaTeacherRequest request)
        {
            try
            {
                var result = await _halqaCommandService.ReassignTeacherAsync(request);
                if (!result.Success)
                    return StatusCode(result.StatusCode, result);

                return Ok(result);
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

        [HttpPut("move-to-fouj")]
        public async Task<ActionResult<GeneralResponse>> MoveToFouj([FromBody] MoveHalqaToFoujRequest request)
        {
            try
            {
                var result = await _halqaCommandService.MoveToFoujAsync(request);
                if (!result.Success)
                    return StatusCode(result.StatusCode, result);

                return Ok(result);
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
