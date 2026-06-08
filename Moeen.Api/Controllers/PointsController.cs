using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Points;
using Moeen.Shared.Responses;
using System;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointsController : ControllerBase
    {
        private readonly IPointsService _pointsService;

        public PointsController(IPointsService pointsService)
        {
            _pointsService = pointsService;
        }

        /// <summary>
        /// ≈⁄œ«œ ‰Ÿ«„ «·‰ﬁ«ÿ
        /// </summary>
        [HttpPost("setup")]
        public async Task<ActionResult<GeneralResponse>> SetupPointsSystem([FromBody] SetupPointsSystemRequest request)
        {
            var response = await _pointsService.SetupPointsSystemAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// «·Õ’Ê· ⁄·Ï —’Ìœ ‰ﬁ«ÿ «·ÿ«·»
        /// </summary>
        [HttpGet("students/{studentId:guid}/points")]
        public async Task<ActionResult<GeneralResponse>> GetStudentPoints([FromRoute] Guid studentId)
        {
            var response = await _pointsService.GetStudentPointsAsync(new GetStudentPointsRequest
            {
                StudentId = studentId
            });

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        ///  ›’Ì· ‰ﬁ«ÿ «·ÿ«·»
        /// </summary>
        [HttpGet("students/{studentId:guid}/points-breakdown")]
        public async Task<ActionResult<GeneralResponse>> GetStudentPointsBreakdown(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var response = await _pointsService.GetStudentPointsBreakdownAsync(new GetStudentPointsBreakdownRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate
            });

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Ã·» ·ÊÕ… «·’œ«—…
        /// </summary>
        [HttpGet("leaderboard")]
        public async Task<ActionResult<GeneralResponse>> GetLeaderboard([FromQuery] GetLeaderboardRequest request)
        {
            var response = await _pointsService.GetPointsLeaderboardAsync(request ?? new GetLeaderboardRequest());
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// „‰Õ ‰ﬁ«ÿ ÌœÊÌ« ·ÿ«·»
        /// </summary>
        [HttpPost("award")]
        public async Task<ActionResult<GeneralResponse>> AwardPointsManually([FromBody] AwardPointsManualRequest request)
        {
            var response = await _pointsService.AwardPointsManuallyAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Œ’„ ‰ﬁ«ÿ „‰ ÿ«·»
        /// </summary>
        [HttpPost("remove")]
        public async Task<ActionResult<GeneralResponse>> RemovePointsManually([FromBody] RemovePointsManualRequest request)
        {
            var response = await _pointsService.RemovePointsManuallyAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// «Õ ”«» «·‰ﬁ«ÿ «· ·ﬁ«∆Ì…
        /// </summary>
        [HttpPost("automatic-award")]
        public async Task<ActionResult<GeneralResponse>> EvaluateAutomaticPoints([FromBody] EvaluateAutomaticPointsRequest request)
        {
            var response = await _pointsService.EvaluateAutomaticPointsAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}