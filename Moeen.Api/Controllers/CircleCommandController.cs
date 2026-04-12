using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Circle;
using Moeen.Api.Shared.Responses.Circle;

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
        /// أمر: إنشاء حلقة جديدة.
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<CircleDto>> CreateCircle(CreateCircleRequest request)
            => Ok(await _circleCommandService.CreateCircleAsync(request));

        /// <summary>
        /// أمر: تحديث حلقة.
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<CircleDto>> UpdateCircle(UpdateCircleRequest request)
            => Ok(await _circleCommandService.UpdateCircleAsync(request));

        /// <summary>
        /// أمر: حذف حلقة.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<ActionResult<bool>> DeleteCircle(DeleteCircleRequest request)
            => Ok(await _circleCommandService.DeleteCircleAsync(request));
    }
}