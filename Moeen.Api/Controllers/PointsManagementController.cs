using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Points;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.Points;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/points-management")]
    [ApiController]
    public class PointsManagementController : ControllerBase
    {
        private readonly IPointsService _pointsService;

        public PointsManagementController(IPointsService pointsService)
        {
            _pointsService = pointsService;
        }

        [HttpPost("setup-system")]
        public async Task<ActionResult<SetupPointsSystemResponse>> SetupPointsSystem([FromBody] SetupPointsSystemRequest request)
            => Ok(await _pointsService.SetupPointsSystemAsync(request));

        [HttpGet("students/{studentId:guid}")]
        public async Task<ActionResult<GetStudentPointsResponse>> GetStudentPoints([FromRoute] Guid studentId)
            => Ok(await _pointsService.GetStudentPointsAsync(new GetStudentPointsRequest { StudentId = studentId }));

        [HttpGet("students/{studentId:guid}/history")]
        public async Task<ActionResult<GetPointsHistoryResponse>> GetPointsHistory([FromRoute] Guid studentId)
            => Ok(await _pointsService.GetPointsHistoryAsync(new GetPointsHistoryRequest { StudentId = studentId }));

        [HttpGet("transactions/{transactionId:guid}")]
        public async Task<ActionResult<PointsTransactionDto>> GetTransactionById([FromRoute] Guid transactionId)
            => Ok(await _pointsService.GetTransactionByIdAsync(new GetTransactionByIdRequest { TransactionId = transactionId }));

        [HttpGet("rules")]
        public async Task<ActionResult<List<PointRuleDto>>> GetPointRules([FromQuery] bool includeInactive = false)
            => Ok(await _pointsService.GetPointRulesAsync(new GetPointRulesRequest { IncludeInactive = includeInactive }));

        [HttpPut("rules")]
        public async Task<ActionResult<PointRuleDto>> UpdatePointRule([FromBody] UpdatePointRuleRequest request)
            => Ok(await _pointsService.UpdatePointRuleAsync(request));

        [HttpDelete("rules/{ruleId:guid}")]
        public async Task<ActionResult<OperationResponseDto>> DeletePointRule([FromRoute] Guid ruleId)
            => Ok(await _pointsService.DeletePointRuleAsync(new DeletePointRuleRequest { RuleId = ruleId }));

        [HttpGet("leaderboard")]
        public async Task<ActionResult<PagedList<StudentLeaderboardDto>>> GetPointsLeaderboard(
            [FromQuery] Guid? circleId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
            => Ok(await _pointsService.GetPointsLeaderboardAsync(new GetLeaderboardRequest
            {
                CircleId = circleId,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));

        [HttpGet("types")]
        public async Task<ActionResult<List<PointTypeDto>>> GetPointTypes([FromQuery] bool includeInactive = true)
            => Ok(await _pointsService.GetPointTypesAsync(new GetPointTypesRequest { IncludeInactive = includeInactive }));

        [HttpGet("circles/{circleId:guid}/summary")]
        public async Task<ActionResult<List<StudentPointsSummaryDto>>> GetCirclePointsSummary([FromRoute] Guid circleId)
            => Ok(await _pointsService.GetCirclePointsSummaryAsync(new GetCirclePointsSummaryRequest { CircleId = circleId }));

        [HttpPost("award-manual")]
        public async Task<ActionResult<PointsTransactionDto>> AwardPointsManually([FromBody] AwardPointsManualRequest request)
            => Ok(await _pointsService.AwardPointsManuallyAsync(request));
    }
}