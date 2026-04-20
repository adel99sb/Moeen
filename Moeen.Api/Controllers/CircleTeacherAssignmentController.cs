using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using System;
using System.Collections.Generic;
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
        /// أمر: تعيين معلم مسؤول عن حلقة.
        /// </summary>
        [HttpPost("assign-teacher")]
        public async Task<ActionResult<OperationResponseDto>> AssignTeacherToCircle([FromBody] AssignTeacherToCircleRequest request)
            => Ok(await _assignmentService.AssignTeacherToCircleAsync(request));

        /// <summary>
        /// أمر: إزالة معلم من الإشراف على حلقة.
        /// </summary>
        [HttpPost("remove-teacher")]
        public async Task<ActionResult<OperationResponseDto>> RemoveTeacherFromCircle([FromBody] RemoveTeacherFromCircleRequest request)
            => Ok(await _assignmentService.RemoveTeacherFromCircleAsync(request));

        /// <summary>
        /// GET: جلب الحلقات المسندة لمعلم محدد.
        /// </summary>
        [HttpGet("teachers/{teacherId:guid}/circles")]
        public async Task<ActionResult<List<CircleAssignmentDto>>> GetCirclesByTeacher(
            [FromRoute] Guid teacherId,
            [FromQuery] bool includeHistory = false)
        {
            var request = new GetCirclesByTeacherRequest
            {
                TeacherId = teacherId,
                IncludeHistory = includeHistory
            };

            return Ok(await _assignmentService.GetCirclesByTeacherAsync(request));
        }

        /// <summary>
        /// GET: جلب المعلمين المسندين لحلقة محددة.
        /// </summary>
        [HttpGet("circles/{circleId:guid}/teachers")]
        public async Task<ActionResult<List<CircleAssignmentDto>>> GetTeachersByCircle(
            [FromRoute] Guid circleId,
            [FromQuery] bool includeInactive = false)
        {
            var request = new GetTeachersByCircleRequest
            {
                CircleId = circleId,
                IncludeInactive = includeInactive
            };

            return Ok(await _assignmentService.GetTeachersByCircleAsync(request));
        }

        /// <summary>
        /// PUT: استبدال معلم بآخر في نفس الحلقة.
        /// </summary>
        [HttpPut("replace-teacher")]
        public async Task<ActionResult<OperationResponseDto>> ReplaceTeacherInCircle([FromBody] ReplaceTeacherRequest request)
            => Ok(await _assignmentService.ReplaceTeacherInCircleAsync(request));
    }
}