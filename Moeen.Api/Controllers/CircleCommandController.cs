using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Circle;
using Moeen.Api.Shared.Responses.Circle;
using System.Threading.Tasks;

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
        /// إنشاء حلقة جديدة
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<CircleDto>> CreateCircle(CreateCircleRequest request)
        {
            var result = await _circleCommandService.CreateCircleAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات حلقة موجودة
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<CircleDto>> UpdateCircle(UpdateCircleRequest request)
        {
            var result = await _circleCommandService.UpdateCircleAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// حذف حلقة
        /// </summary>
        [HttpDelete("delete")]
        public async Task<ActionResult<bool>> DeleteCircle(DeleteCircleRequest request)
        {
            var result = await _circleCommandService.DeleteCircleAsync(request);
            return Ok(result);
        }
    }
}