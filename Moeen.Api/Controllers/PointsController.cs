using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Points;
using Moeen.Api.Shared.Responses.Points;
using System.Threading.Tasks;

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
        {
            var result = await _pointsService.SetupPointsSystemAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على رصيد نقاط طالب
        /// </summary>
        [HttpPost("student-points")]
        public async Task<ActionResult<GetStudentPointsResponse>> GetStudentPoints(GetStudentPointsRequest request)
        {
            var result = await _pointsService.GetStudentPointsAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على سجل نقاط طالب
        /// </summary>
        [HttpPost("points-history")]
        public async Task<ActionResult<GetPointsHistoryResponse>> GetPointsHistory(GetPointsHistoryRequest request)
        {
            var result = await _pointsService.GetPointsHistoryAsync(request);
            return Ok(result);
        }
    }
}