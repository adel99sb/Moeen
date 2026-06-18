using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.HalqaQuery;
using Moeen.Shared.Responses;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HalqaQueryController : ControllerBase
    {
        private readonly IHalqaQueryService _halqaQueryService;

        public HalqaQueryController(IHalqaQueryService halqaQueryService)
        {
            _halqaQueryService = halqaQueryService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<GeneralResponse>> GetAllHalqas([FromQuery] Guid? mosqueId)
        {
            try
            {
                var result = await _halqaQueryService.GetAllHalqasAsync(mosqueId);
                return Ok(GeneralResponse.Ok("تم جلب الحلقات بنجاح.", result));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب الحلقات"));
            }
        }

        [HttpGet("assignment-students")]
        public async Task<ActionResult<GeneralResponse>> GetAssignmentStudents([FromQuery] Guid? halqaId)
        {
            try
            {
                var result = await _halqaQueryService.GetAssignmentStudentsAsync(halqaId);
                return Ok(GeneralResponse.Ok("تم جلب الطلاب المتاحين بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب الطلاب المتاحين"));
            }
        }

        [HttpPost("get-by-id")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaById([FromBody] GetHalqaByIdRequest request)
        {
            try
            {
                var result = await _halqaQueryService.GetHalqaByIdAsync(request);
                return Ok(GeneralResponse.Ok("تم جلب بيانات الحلقة بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب بيانات الحلقة"));
            }
        }

        [HttpGet("{HalqaId:guid}")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaByIdGet([FromRoute] Guid HalqaId)
        {
            try
            {
                var result = await _halqaQueryService.GetHalqaByIdAsync(new GetHalqaByIdRequest { HalqaId = HalqaId });
                return Ok(GeneralResponse.Ok("تم جلب بيانات الحلقة بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب البيانات"));
            }
        }

        [HttpPost("get-students")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaStudents([FromBody] GetHalqaStudentsRequest request)
        {
            try
            {
                var result = await _halqaQueryService.GetHalqaStudentsAsync(request);
                return Ok(GeneralResponse.Ok("تم جلب الطلاب بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب الطلاب"));
            }
        }

        [HttpGet("{HalqaId:guid}/students")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaStudentsGet([FromRoute] Guid HalqaId)
        {
            try
            {
                var result = await _halqaQueryService.GetHalqaStudentsAsync(new GetHalqaStudentsRequest { HalqaId = HalqaId });
                return Ok(GeneralResponse.Ok("تم جلب الطلاب بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب الطلاب"));
            }
        }

        [HttpPost("get-students-count")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaStudentsCount(GetHalqaStudentsCountRequest request)
        {
            try
            {
                var result = await _halqaQueryService.GetHalqaStudentsCountAsync(request);
                return Ok(GeneralResponse.Ok("تم جلب عدد الطلاب بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب عدد الطلاب"));
            }
        }

        [HttpGet("{HalqaId:guid}/students/count")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaStudentsCountGet([FromRoute] Guid HalqaId)
        {
            try
            {
                var result = await _halqaQueryService.GetHalqaStudentsCountAsync(new GetHalqaStudentsCountRequest { HalqaId = HalqaId });
                return Ok(GeneralResponse.Ok("تم جلب عدد الطلاب بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب عدد الطلاب"));
            }
        }

        [HttpPost("statistics")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaStatistics([FromBody] GetHalqaStatisticsRequest request)
        {
            try
            {
                var result = await _halqaQueryService.GetHalqaStatisticsAsync(request);
                return Ok(GeneralResponse.Ok("تم جلب إحصائيات الحلقة بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب الإحصائيات"));
            }
        }

        [HttpGet("{HalqaId:guid}/statistics")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaStatisticsGet(
            [FromRoute] Guid HalqaId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var result = await _halqaQueryService.GetHalqaStatisticsAsync(new GetHalqaStatisticsRequest
                {
                    HalqaId = HalqaId,
                    FromDate = fromDate,
                    ToDate = toDate
                });
                return Ok(GeneralResponse.Ok("تم جلب إحصائيات الحلقة بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب الإحصائيات"));
            }
        }

        [HttpPost("attendance-report")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaAttendanceReport([FromBody] GetHalqaAttendanceReportRequest request)
        {
            try
            {
                var result = await _halqaQueryService.GetHalqaAttendanceReportAsync(request);
                return Ok(GeneralResponse.Ok("تم جلب تقرير الحضور بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب تقرير الحضور"));
            }
        }

        [HttpGet("{HalqaId:guid}/attendance-report")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaAttendanceReportGet(
            [FromRoute] Guid HalqaId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                var result = await _halqaQueryService.GetHalqaAttendanceReportAsync(new GetHalqaAttendanceReportRequest
                {
                    HalqaId = HalqaId,
                    FromDate = fromDate,
                    ToDate = toDate
                });
                return Ok(GeneralResponse.Ok("تم جلب تقرير الحضور بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب تقرير الحضور"));
            }
        }
    }
}
