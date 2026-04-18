using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Scheduling;
using Moeen.Api.Shared.Responses.Scheduling;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulingController : ControllerBase
    {
        private readonly ISchedulingService _schedulingService;

        public SchedulingController(ISchedulingService schedulingService)
        {
            _schedulingService = schedulingService;
        }

        /// <summary>
        /// أمر: إنشاء جدول حلقة.
        /// </summary>
        [HttpPost("create-circle-schedule")]
        public async Task<ActionResult<ScheduleResponseDto>> CreateCircleSchedule(CreateCircleScheduleRequest request)
            => Ok(await _schedulingService.CreateCircleScheduleAsync(request));

        /// <summary>
        /// أمر: تعيين جدول لمعلم.
        /// </summary>
        [HttpPost("assign-to-teacher")]
        public async Task<ActionResult<AssignScheduleToTeacherResponse>> AssignScheduleToTeacher(AssignScheduleToTeacherRequest request)
            => Ok(await _schedulingService.AssignScheduleToTeacherAsync(request));
    }
}