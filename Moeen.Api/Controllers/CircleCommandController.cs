using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Circle;
using Moeen.Shared.Responses.Circle;

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
        public async Task<ActionResult<CircleDto>> CreateCircle([FromBody] CreateCircleRequest request)
            => Ok(await _circleCommandService.CreateCircleAsync(request));

        /// <summary>
        /// أمر: تحديث بيانات حلقة.
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<CircleDto>> UpdateCircle([FromBody] UpdateCircleRequest request)
            => Ok(await _circleCommandService.UpdateCircleAsync(request));

        /// <summary>
        /// أمر: حذف حلقة.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<ActionResult<bool>> DeleteCircle([FromBody] DeleteCircleRequest request)
            => Ok(await _circleCommandService.DeleteCircleAsync(request));

        /// <summary>
        /// أمر: إعادة تعيين معلم للحلقة.
        /// </summary>
        [HttpPut("reassign-teacher")]
        public async Task<ActionResult<CircleDto>> ReassignTeacher([FromBody] ReassignCircleTeacherRequest request)
            => Ok(await _circleCommandService.ReassignTeacherAsync(request));

        /// <summary>
        /// أمر: نقل الحلقة إلى فوج آخر.
        /// </summary>
        [HttpPut("move-to-fouj")]
        public async Task<ActionResult<CircleDto>> MoveToFouj([FromBody] MoveCircleToFoujRequest request)
            => Ok(await _circleCommandService.MoveToFoujAsync(request));
    }
}