using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Scheduling;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.Scheduling;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/scheduling-management")]
    [ApiController]
    public class SchedulingManagementController : ControllerBase
    {
        private readonly ISchedulingService _schedulingService;

        public SchedulingManagementController(ISchedulingService schedulingService)
        {
            _schedulingService = schedulingService;
        }

        [HttpPost("create-circle-schedule")]
        public async Task<ActionResult<ScheduleResponseDto>> CreateCircleSchedule([FromBody] CreateCircleScheduleRequest request)
            => Ok(await _schedulingService.CreateCircleScheduleAsync(request));

        [HttpPost("assign-to-teacher")]
        public async Task<ActionResult<AssignScheduleToTeacherResponse>> AssignScheduleToTeacher([FromBody] AssignScheduleToTeacherRequest request)
            => Ok(await _schedulingService.AssignScheduleToTeacherAsync(request));

        [HttpGet("schedules/{scheduleId:guid}")]
        public async Task<ActionResult<ScheduleResponseDto>> GetCircleScheduleById([FromRoute] Guid scheduleId)
            => Ok(await _schedulingService.GetCircleScheduleByIdAsync(new GetCircleScheduleByIdRequest { ScheduleId = scheduleId }));

        [HttpGet("schedules")]
        public async Task<ActionResult<PagedList<ScheduleResponseDto>>> GetAllSchedules([FromQuery] GetAllSchedulesRequest request)
            => Ok(await _schedulingService.GetAllSchedulesAsync(request));

        [HttpPut("schedules/{scheduleId:guid}")]
        public async Task<ActionResult<ScheduleResponseDto>> UpdateCircleSchedule(
            [FromRoute] Guid scheduleId,
            [FromBody] UpdateCircleScheduleRequest request)
        {
            request.ScheduleId = scheduleId;
            return Ok(await _schedulingService.UpdateCircleScheduleAsync(request));
        }

        [HttpDelete("schedules/{scheduleId:guid}")]
        public async Task<ActionResult<ScheduleOperationResponse>> DeleteSchedule([FromRoute] Guid scheduleId)
            => Ok(await _schedulingService.DeleteScheduleAsync(new DeleteScheduleRequest { ScheduleId = scheduleId }));
    }
}