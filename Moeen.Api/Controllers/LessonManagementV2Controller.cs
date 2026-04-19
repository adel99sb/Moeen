using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.LessonManagement;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.LessonManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/lesson-management-v2")]
    [ApiController]
    public class LessonManagementV2Controller : ControllerBase
    {
        private readonly ILessonManagementService _lessonService;

        public LessonManagementV2Controller(ILessonManagementService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet("{lessonId:guid}")]
        public async Task<ActionResult<LessonDto>> GetLessonById([FromRoute] Guid lessonId)
            => Ok(await _lessonService.GetLessonByIdAsync(new GetLessonByIdRequest { LessonId = lessonId }));

        [HttpGet("circles/{circleId:guid}")]
        public async Task<ActionResult<PagedList<LessonDto>>> GetLessonsByCircle(
            [FromRoute] Guid circleId,
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
            => Ok(await _lessonService.GetLessonsByCircleAsync(new GetLessonsByCircleRequest
            {
                CircleId = circleId,
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));

        [HttpGet("{lessonId:guid}/materials")]
        public async Task<ActionResult<List<LessonMaterialDto>>> GetLessonMaterials([FromRoute] Guid lessonId)
            => Ok(await _lessonService.GetLessonMaterialsAsync(new GetLessonMaterialsRequest { LessonId = lessonId }));

        [HttpGet("circles/{circleId:guid}/schedule")]
        public async Task<ActionResult<LessonScheduleDto>> GetLessonSchedule([FromRoute] Guid circleId)
            => Ok(await _lessonService.GetLessonScheduleAsync(new GetLessonScheduleRequest { CircleId = circleId }));

        [HttpPut("schedule")]
        public async Task<ActionResult<LessonScheduleDto>> UpdateLessonSchedule([FromBody] UpdateLessonScheduleRequest request)
            => Ok(await _lessonService.UpdateLessonScheduleAsync(request));

        [HttpPut("assign-to-circles")]
        public async Task<ActionResult<OperationResponseDto>> AssignLessonToCircles([FromBody] AssignLessonToCirclesRequest request)
            => Ok(await _lessonService.AssignLessonToCirclesAsync(request));

        [HttpPut("assign-teacher")]
        public async Task<ActionResult<OperationResponseDto>> AssignTeacherToLesson([FromBody] AssignTeacherToLessonRequest request)
            => Ok(await _lessonService.AssignTeacherToLessonAsync(request));

        [HttpDelete("soft-delete")]
        public async Task<ActionResult<OperationResponseDto>> SoftDeleteLesson([FromBody] SoftDeleteLessonRequest request)
            => Ok(await _lessonService.SoftDeleteLessonAsync(request));

        [HttpDelete("unassign-from-circle")]
        public async Task<ActionResult<OperationResponseDto>> UnassignLessonFromCircle([FromBody] UnassignLessonFromCircleRequest request)
            => Ok(await _lessonService.UnassignLessonFromCircleAsync(request));

        [HttpPost("duplicate")]
        public async Task<ActionResult<LessonDto>> DuplicateLesson([FromBody] DuplicateLessonRequest request)
            => Ok(await _lessonService.DuplicateLessonAsync(request));

        [HttpPost("record-attendance")]
        public async Task<ActionResult<LessonAttendanceDto>> RecordLessonAttendance([FromBody] RecordAttendanceRequest request)
            => Ok(await _lessonService.RecordLessonAttendanceAsync(request));
    }
}