using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using System.Threading.Tasks;

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
        /// تعيين معلم مسؤول عن حلقة
        /// </summary>
        [HttpPost("assign-teacher")]
        public async Task<ActionResult<OperationResponse>> AssignTeacherToCircle(AssignTeacherToCircleRequest request)
        {
            var result = await _assignmentService.AssignTeacherToCircleAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// إزالة معلم من الإشراف على حلقة
        /// </summary>
        [HttpPost("remove-teacher")]
        public async Task<ActionResult<OperationResponse>> RemoveTeacherFromCircle(RemoveTeacherFromCircleRequest request)
        {
            var result = await _assignmentService.RemoveTeacherFromCircleAsync(request);
            return Ok(result);
        }
    }
}