using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.CircleQuery;
using Moeen.Shared.Responses;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircleQueryController : ControllerBase
    {
        private readonly ICircleQueryService _circleQueryService;

        public CircleQueryController(ICircleQueryService circleQueryService)
        {
            _circleQueryService = circleQueryService;
        }

        /// <summary>
        /// POST (قديم/متوافق): جلب تفاصيل حلقة عبر Body.
        /// </summary>
        [HttpPost("get-by-id")]
        public async Task<ActionResult<GeneralResponse>> GetCircleById([FromBody] GetCircleByIdRequest request)
        {
            try
            {
                var result = await _circleQueryService.GetCircleByIdAsync(request);
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
        [HttpGet("{circleId:guid}")]
        public async Task<ActionResult<GeneralResponse>> GetCircleByIdGet([FromRoute] Guid circleId)
        {
            try
            {
                var result = await _circleQueryService.GetCircleByIdAsync(new GetCircleByIdRequest { CircleId = circleId });
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
        public async Task<ActionResult<GeneralResponse>> GetCircleStudents([FromBody] GetCircleStudentsRequest request)
        {
            try
            {
                var result = await _circleQueryService.GetCircleStudentsAsync(request);
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
        [HttpGet("{circleId:guid}/students")]
        public async Task<ActionResult<GeneralResponse>> GetCircleStudentsGet([FromRoute] Guid circleId)
        {
            try
            {
                var result = await _circleQueryService.GetCircleStudentsAsync(new GetCircleStudentsRequest { CircleId = circleId });
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
        public async Task<ActionResult<GeneralResponse>> GetCircleStudentsCount(GetCircleStudentsCountRequest request)
        {
            try
            {
                var result = await _circleQueryService.GetCircleStudentsCountAsync(request);
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
        [HttpGet("{circleId:guid}/students/count")]
        public async Task<ActionResult<GeneralResponse>> GetCircleStudentsCountGet([FromRoute] Guid circleId)
        {
            try
            {
                var result = await _circleQueryService.GetCircleStudentsCountAsync(new GetCircleStudentsCountRequest { CircleId = circleId });
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
        public async Task<ActionResult<GeneralResponse>> GetCircleStatistics([FromBody] GetCircleStatisticsRequest request)
        {
            try
            {
                var result = await _circleQueryService.GetCircleStatisticsAsync(request);
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
        [HttpGet("{circleId:guid}/statistics")]
        public async Task<ActionResult<GeneralResponse>> GetCircleStatisticsGet(
            [FromRoute] Guid circleId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var result = await _circleQueryService.GetCircleStatisticsAsync(new GetCircleStatisticsRequest
                {
                    CircleId = circleId,
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
        public async Task<ActionResult<GeneralResponse>> GetCircleAttendanceReport([FromBody] GetCircleAttendanceReportRequest request)
        {
            try
            {
                var result = await _circleQueryService.GetCircleAttendanceReportAsync(request);
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
        [HttpGet("{circleId:guid}/attendance-report")]
        public async Task<ActionResult<GeneralResponse>> GetCircleAttendanceReportGet(
            [FromRoute] Guid circleId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                var result = await _circleQueryService.GetCircleAttendanceReportAsync(new GetCircleAttendanceReportRequest
                {
                    CircleId = circleId,
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