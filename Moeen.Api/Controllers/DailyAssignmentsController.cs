using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.DailyAssignments;
using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/daily-assignments")]
    [ApiController]
    public class DailyAssignmentsController : ControllerBase
    {
        private readonly IDailyAssignmentService _dailyAssignmentService;

        public DailyAssignmentsController(IDailyAssignmentService dailyAssignmentService)
        {
            _dailyAssignmentService = dailyAssignmentService;
        }

        [HttpGet("students/{studentId:guid}")]
        public async Task<ActionResult<GeneralResponse>> GetDailyAssignments(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime? date,
            [FromQuery] Guid? halqaId)
        {
            var response = await _dailyAssignmentService.GetDailyAssignmentsAsync(new GetDailyAssignmentsRequest
            {
                StudentId = studentId,
                Date = date,
                HalqaId = halqaId
            });

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> AddDailyAssignment([FromBody] AddDailyAssignmentRequest request)
        {
            var response = await _dailyAssignmentService.AddDailyAssignmentAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("status")]
        public async Task<ActionResult<GeneralResponse>> UpdateAssignmentStatus([FromBody] UpdateAssignmentStatusRequest request)
        {
            var response = await _dailyAssignmentService.UpdateAssignmentStatusAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}