using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Points;
using Moeen.Api.Shared.Responses.Points;
using System;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointsController : ControllerBase
    {
        private readonly IPointsService _pointsService;

        public PointsController(IPointsService pointsService)
        {
            _pointsService = pointsService;
        }

        /// <summary>
        /// إعداد نظام النقاط (تعيين النقاط لكل تقدير)
        /// </summary>
        [HttpPost("setup-system")]
        public async Task<ActionResult<SetupPointsSystemResponse>> SetupPointsSystem(SetupPointsSystemRequest request)
            => Ok(await _pointsService.SetupPointsSystemAsync(request));

        /// <summary>
        /// الحصول على رصيد نقاط طالب
        /// </summary>
        [HttpPost("student-points")]
        public async Task<ActionResult<GetStudentPointsResponse>> GetStudentPoints(GetStudentPointsRequest request)
            => Ok(await _pointsService.GetStudentPointsAsync(request));

        /// <summary>
        /// GET (جديد): رصيد نقاط الطالب.
        /// </summary>
        [HttpGet("students/{studentId:guid}")]
        public async Task<ActionResult<GetStudentPointsResponse>> GetStudentPointsGet([FromRoute] Guid studentId)
            => Ok(await _pointsService.GetStudentPointsAsync(new GetStudentPointsRequest { StudentId = studentId }));

        /// <summary>
        /// الحصول على سجل نقاط طالب
        /// </summary>
        [HttpPost("points-history")]
        public async Task<ActionResult<GetPointsHistoryResponse>> GetPointsHistory(GetPointsHistoryRequest request)
            => Ok(await _pointsService.GetPointsHistoryAsync(request));

        /// <summary>
        /// GET (جديد): سجل نقاط الطالب.
        /// </summary>
        [HttpGet("students/{studentId:guid}/history")]
        public async Task<ActionResult<GetPointsHistoryResponse>> GetPointsHistoryGet([FromRoute] Guid studentId)
            => Ok(await _pointsService.GetPointsHistoryAsync(new GetPointsHistoryRequest { StudentId = studentId }));
    }
}