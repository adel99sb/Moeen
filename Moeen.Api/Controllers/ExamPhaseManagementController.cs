using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamPhase;
using Moeen.Api.Shared.Responses.ExamPhase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/exam-phases")]
    [ApiController]
    public class ExamPhaseManagementController : ControllerBase
    {
        private readonly IExamPhaseService _examPhaseService;

        public ExamPhaseManagementController(IExamPhaseService examPhaseService)
        {
            _examPhaseService = examPhaseService;
        }

        /// <summary>
        /// ≈‰‘«¡ „—Õ·… «Œ »«—Ì… ÃœÌœ…
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ExamPhaseDto>> DefineExamPhase([FromBody] DefineExamPhaseRequest request)
            => Ok(await _examPhaseService.DefineExamPhaseAsync(request));

        /// <summary>
        /// GET: Ã·» ﬂ· «·„—«Õ· „⁄ «· ’›Õ
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<GetExamPhasesResponse>> GetExamPhases(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "PhaseName",
            [FromQuery] bool sortDescending = false)
            => Ok(await _examPhaseService.GetExamPhasesAsync(new GetExamPhasesRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending
            }));

        /// <summary>
        /// GET: Ã·» „—Õ·… »Ê«”ÿ… «·„⁄—›
        /// </summary>
        [HttpGet("{phaseId:guid}")]
        public async Task<ActionResult<ExamPhaseDto>> GetExamPhaseById([FromRoute] Guid phaseId)
            => Ok(await _examPhaseService.GetExamPhaseByIdAsync(new GetExamPhaseByIdRequest { PhaseId = phaseId }));

        /// <summary>
        /// GET: Ã·» «·„—«Õ· «·„— »ÿ… »Õ·ﬁ… „⁄Ì‰…
        /// </summary>
        [HttpGet("by-circle/{circleId:guid}")]
        public async Task<ActionResult<List<ExamPhaseDto>>> GetExamPhasesByCircle([FromRoute] Guid circleId)
            => Ok(await _examPhaseService.GetExamPhasesByCircleAsync(new GetExamPhasesByCircleRequest { CircleId = circleId }));

        /// <summary>
        /// PUT:  ÕœÌÀ „⁄·Ê„«  „—Õ·…
        /// </summary>
        [HttpPut("{phaseId:guid}")]
        public async Task<ActionResult<ExamPhaseDto>> UpdateExamPhaseInfo(
            [FromRoute] Guid phaseId,
            [FromBody] UpdateExamPhaseInfoRequest request)
        {
            request.PhaseId = phaseId;
            return Ok(await _examPhaseService.UpdateExamPhaseInfoAsync(request));
        }

        /// <summary>
        /// DELETE: Õ–› „—Õ·…
        /// </summary>
        [HttpDelete("{phaseId:guid}")]
        public async Task<ActionResult<DeleteExamPhaseResponse>> DeleteExamPhase([FromRoute] Guid phaseId)
            => Ok(await _examPhaseService.DeleteExamPhaseAsync(new DeleteExamPhaseRequest { PhaseId = phaseId }));
    }
}