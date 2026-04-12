using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircleTeacherAssignmentController : ControllerBase
    {
        private readonly ICircleTeacherAssignmentService _assignmentService;

        public CircleTeacherAssignmentController(ICircleTeacherAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        /// <summary>
        /// أمر: تعيين معلم لحلقة.
        /// </summary>
        [HttpPost("assign-teacher")]
        public async Task<ActionResult<OperationResponse>> AssignTeacherToCircle(AssignTeacherToCircleRequest request)
            => Ok(await _assignmentService.AssignTeacherToCircleAsync(request));

        /// <summary>
        /// أمر: إزالة معلم من حلقة.
        /// </summary>
        [HttpPost("remove-teacher")]
        public async Task<ActionResult<OperationResponse>> RemoveTeacherFromCircle(RemoveTeacherFromCircleRequest request)
            => Ok(await _assignmentService.RemoveTeacherFromCircleAsync(request));
    }
}