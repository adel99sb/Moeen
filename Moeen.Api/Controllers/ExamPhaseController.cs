using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamPhase;
using Moeen.Api.Shared.Responses.ExamPhase;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamPhaseController : ControllerBase
    {
        private readonly IExamPhaseService _examPhaseService;

        public ExamPhaseController(IExamPhaseService examPhaseService)
        {
            _examPhaseService = examPhaseService;
        }

        /// <summary>
        /// أمر: تعريف مرحلة اختبارية جديدة.
        /// </summary>
        [HttpPost("define")]
        public async Task<ActionResult<ExamPhaseDto>> DefineExamPhase(DefineExamPhaseRequest request)
            => Ok(await _examPhaseService.DefineExamPhaseAsync(request));

        /// <summary>
        /// POST (قديم/متوافق): جلب جميع المراحل.
        /// </summary>
        [HttpPost("get-all")]
        public async Task<ActionResult<GetExamPhasesResponse>> GetExamPhases(GetExamPhasesRequest request)
            => Ok(await _examPhaseService.GetExamPhasesAsync(request));

        /// <summary>
        /// GET (جديد): جلب جميع المراحل عبر Query Parameters.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<GetExamPhasesResponse>> GetExamPhasesGet(
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
        /// أمر: حذف مرحلة.
        /// </summary>
        [HttpPost("delete")]
        public async Task<ActionResult<DeleteExamPhaseResponse>> DeleteExamPhase(DeleteExamPhaseRequest request)
            => Ok(await _examPhaseService.DeleteExamPhaseAsync(request));
    }
}