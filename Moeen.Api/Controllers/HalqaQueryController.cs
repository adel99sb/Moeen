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
        private readonly IHalqaQueryService _HalqaQueryService;

        public HalqaQueryController(IHalqaQueryService HalqaQueryService)
        {
            _HalqaQueryService = HalqaQueryService;
        }

        /// <summary>
        /// POST (قديم/متوافق): جلب تفاصيل حلقة عبر Body.
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<GeneralResponse>> GetAllHalqas([FromQuery] Guid? mosqueId)
        {
            try
            {
                var result = await _HalqaQueryService.GetAllHalqasAsync(mosqueId);
                return Ok(GeneralResponse.Ok("تم جلب الحلقات بنجاح.", result));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب الحلقات"));
            }
        }

        [HttpPost("get-by-id")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaById([FromBody] GetHalqaByIdRequest request)
        {
            try
            {
                var result = await _HalqaQueryService.GetHalqaByIdAsync(request);
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

        /// <summary>
        /// GET (جديد): جلب تفاصيل حلقة عبر Route.
        /// </summary>
        [HttpGet("{HalqaId:guid}")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaByIdGet([FromRoute] Guid HalqaId)
        {
            try
            {
                var result = await _HalqaQueryService.GetHalqaByIdAsync(new GetHalqaByIdRequest { HalqaId = HalqaId });
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

        /// <summary>
        /// POST (قديم/متوافق): جلب طلاب الحلقة مع التصفية.
        /// </summary>
        [HttpPost("get-students")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaStudents([FromBody] GetHalqaStudentsRequest request)
        {
            try
            {
                var result = await _HalqaQueryService.GetHalqaStudentsAsync(request);
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

        /// <summary>
        /// GET (جديد): جلب طلاب الحلقة (بدون فلترة معقدة).
        /// </summary>
        [HttpGet("{HalqaId:guid}/students")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaStudentsGet([FromRoute] Guid HalqaId)
        {
            try
            {
                var result = await _HalqaQueryService.GetHalqaStudentsAsync(new GetHalqaStudentsRequest { HalqaId = HalqaId });
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

        /// <summary>
        /// POST (قديم/متوافق): جلب عدد طلاب الحلقة.
        /// </summary>
        [HttpPost("get-students-count")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaStudentsCount(GetHalqaStudentsCountRequest request)
        {
            try
            {
                var result = await _HalqaQueryService.GetHalqaStudentsCountAsync(request);
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

        /// <summary>
        /// GET (جديد): جلب عدد طلاب الحلقة عبر Route.
        /// </summary>
        [HttpGet("{HalqaId:guid}/students/count")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaStudentsCountGet([FromRoute] Guid HalqaId)
        {
            try
            {
                var result = await _HalqaQueryService.GetHalqaStudentsCountAsync(new GetHalqaStudentsCountRequest { HalqaId = HalqaId });
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

        /// <summary>
        /// POST (جديد): جلب إحصائيات وتقدم الحلقة.
        /// </summary>
        [HttpPost("statistics")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaStatistics([FromBody] GetHalqaStatisticsRequest request)
        {
            try
            {
                var result = await _HalqaQueryService.GetHalqaStatisticsAsync(request);
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

        /// <summary>
        /// GET (جديد): جلب إحصائيات الحلقة عبر Route + Query.
        /// </summary>
        [HttpGet("{HalqaId:guid}/statistics")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaStatisticsGet(
            [FromRoute] Guid HalqaId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var result = await _HalqaQueryService.GetHalqaStatisticsAsync(new GetHalqaStatisticsRequest
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

        /// <summary>
        /// POST (جديد): جلب تقرير حضور الحلقة خلال فترة.
        /// </summary>
        [HttpPost("attendance-report")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaAttendanceReport([FromBody] GetHalqaAttendanceReportRequest request)
        {
            try
            {
                var result = await _HalqaQueryService.GetHalqaAttendanceReportAsync(request);
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

        /// <summary>
        /// GET (جديد): جلب تقرير حضور الحلقة عبر Route + Query.
        /// </summary>
        [HttpGet("{HalqaId:guid}/attendance-report")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaAttendanceReportGet(
            [FromRoute] Guid HalqaId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                var result = await _HalqaQueryService.GetHalqaAttendanceReportAsync(new GetHalqaAttendanceReportRequest
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