using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.CircleQuery;
using Moeen.Api.Shared.Responses.Circle;
using Moeen.Api.Shared.Responses.CircleQuery;
using System;

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
        public async Task<ActionResult<CircleDto>> GetCircleById(GetCircleByIdRequest request)
            => Ok(await _circleQueryService.GetCircleByIdAsync(request));

        /// <summary>
        /// GET (جديد): جلب تفاصيل حلقة عبر Route.
        /// </summary>
        [HttpGet("{circleId:guid}")]
        public async Task<ActionResult<CircleDto>> GetCircleByIdGet([FromRoute] Guid circleId)
            => Ok(await _circleQueryService.GetCircleByIdAsync(new GetCircleByIdRequest { CircleId = circleId }));

        /// <summary>
        /// POST (قديم/متوافق): جلب طلاب الحلقة مع التصفية.
        /// </summary>
        [HttpPost("get-students")]
        public async Task<ActionResult<CircleStudentsResponse>> GetCircleStudents(GetCircleStudentsRequest request)
            => Ok(await _circleQueryService.GetCircleStudentsAsync(request));

        /// <summary>
        /// GET (جديد): جلب طلاب الحلقة (بدون فلترة معقدة).
        /// </summary>
        [HttpGet("{circleId:guid}/students")]
        public async Task<ActionResult<CircleStudentsResponse>> GetCircleStudentsGet([FromRoute] Guid circleId)
            => Ok(await _circleQueryService.GetCircleStudentsAsync(new GetCircleStudentsRequest { CircleId = circleId }));

        /// <summary>
        /// POST (قديم/متوافق): جلب عدد طلاب الحلقة.
        /// </summary>
        [HttpPost("get-students-count")]
        public async Task<ActionResult<CircleStudentsCountResponse>> GetCircleStudentsCount(GetCircleStudentsCountRequest request)
            => Ok(await _circleQueryService.GetCircleStudentsCountAsync(request));

        /// <summary>
        /// GET (جديد): جلب عدد طلاب الحلقة عبر Route.
        /// </summary>
        [HttpGet("{circleId:guid}/students/count")]
        public async Task<ActionResult<CircleStudentsCountResponse>> GetCircleStudentsCountGet([FromRoute] Guid circleId)
            => Ok(await _circleQueryService.GetCircleStudentsCountAsync(new GetCircleStudentsCountRequest { CircleId = circleId }));

        /// <summary>
        /// POST (جديد): جلب إحصائيات وتقدم الحلقة.
        /// </summary>
        [HttpPost("statistics")]
        public async Task<ActionResult<CircleStatisticsDto>> GetCircleStatistics([FromBody] GetCircleStatisticsRequest request)
            => Ok(await _circleQueryService.GetCircleStatisticsAsync(request));

        /// <summary>
        /// GET (جديد): جلب إحصائيات الحلقة عبر Route + Query.
        /// </summary>
        [HttpGet("{circleId:guid}/statistics")]
        public async Task<ActionResult<CircleStatisticsDto>> GetCircleStatisticsGet(
            [FromRoute] Guid circleId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
            => Ok(await _circleQueryService.GetCircleStatisticsAsync(new GetCircleStatisticsRequest
            {
                CircleId = circleId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        /// <summary>
        /// POST (جديد): جلب تقرير حضور الحلقة خلال فترة.
        /// </summary>
        [HttpPost("attendance-report")]
        public async Task<ActionResult<CircleAttendanceReportResponse>> GetCircleAttendanceReport([FromBody] GetCircleAttendanceReportRequest request)
            => Ok(await _circleQueryService.GetCircleAttendanceReportAsync(request));

        /// <summary>
        /// GET (جديد): جلب تقرير حضور الحلقة عبر Route + Query.
        /// </summary>
        [HttpGet("{circleId:guid}/attendance-report")]
        public async Task<ActionResult<CircleAttendanceReportResponse>> GetCircleAttendanceReportGet(
            [FromRoute] Guid circleId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
            => Ok(await _circleQueryService.GetCircleAttendanceReportAsync(new GetCircleAttendanceReportRequest
            {
                CircleId = circleId,
                FromDate = fromDate,
                ToDate = toDate
            }));
    }
}