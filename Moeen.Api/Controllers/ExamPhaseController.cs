using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamPhase;
using Moeen.Api.Shared.Responses.ExamPhase;
using System.Threading.Tasks;

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
        /// تعريف مرحلة اختبارية جديدة
        /// </summary>
        [HttpPost("define")]
        public async Task<ActionResult<ExamPhaseDto>> DefineExamPhase(DefineExamPhaseRequest request)
        {
            var result = await _examPhaseService.DefineExamPhaseAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على جميع المراحل المعرفة
        /// </summary>
        [HttpPost("get-all")]
        public async Task<ActionResult<GetExamPhasesResponse>> GetExamPhases(GetExamPhasesRequest request)
        {
            var result = await _examPhaseService.GetExamPhasesAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// حذف مرحلة
        /// </summary>
        [HttpPost("delete")]
        public async Task<ActionResult<DeleteExamPhaseResponse>> DeleteExamPhase(DeleteExamPhaseRequest request)
        {
            var result = await _examPhaseService.DeleteExamPhaseAsync(request);
            return Ok(result);
        }
    }
}