using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.CircleQuery;
using Moeen.Api.Shared.Responses.Circle;
using Moeen.Api.Shared.Responses.CircleQuery;
using System.Threading.Tasks;

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
        /// الحصول على تفاصيل حلقة محددة
        /// </summary>
        [HttpPost("get-by-id")]
        public async Task<ActionResult<CircleDto>> GetCircleById(GetCircleByIdRequest request)
        {
            var result = await _circleQueryService.GetCircleByIdAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على قائمة الطلاب المسجلين في حلقة مع التصفية
        /// </summary>
        [HttpPost("get-students")]
        public async Task<ActionResult<CircleStudentsResponse>> GetCircleStudents(GetCircleStudentsRequest request)
        {
            var result = await _circleQueryService.GetCircleStudentsAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على عدد الطلاب المسجلين في حلقة
        /// </summary>
        [HttpPost("get-students-count")]
        public async Task<ActionResult<CircleStudentsCountResponse>> GetCircleStudentsCount(GetCircleStudentsCountRequest request)
        {
            var result = await _circleQueryService.GetCircleStudentsCountAsync(request);
            return Ok(result);
        }
    }
}