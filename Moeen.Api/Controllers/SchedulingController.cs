using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Scheduling;
using Moeen.Api.Shared.Responses.Scheduling;
using System.Threading.Tasks;

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

        [HttpPost("create-circle-schedule")]
        public async Task<ActionResult<ScheduleResponseDto>> CreateCircleSchedule(CreateCircleScheduleRequest request)
        {
            var result = await _schedulingService.CreateCircleScheduleAsync(request);
            return Ok(result);
        }

        [HttpPost("assign-to-teacher")]
        public async Task<ActionResult<AssignScheduleToTeacherResponse>> AssignScheduleToTeacher(AssignScheduleToTeacherRequest request)
        {
            var result = await _schedulingService.AssignScheduleToTeacherAsync(request);
            return Ok(result);
        }
    }
}