using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamGrading;
using Moeen.Api.Shared.Responses.ExamGrading;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamGradingCriteriaController : ControllerBase
    {
        private readonly IExamGradingCriteriaService _gradingCriteriaService;

        public ExamGradingCriteriaController(IExamGradingCriteriaService gradingCriteriaService)
        {
            _gradingCriteriaService = gradingCriteriaService;
        }

        /// <summary>
        /// إضافة أو تحديث معيار تقدير
        /// </summary>
        [HttpPost("set-criteria")]
        public async Task<ActionResult<GradingCriteriaDto>> SetGradingCriteria(SetGradingCriteriaRequest request)
        {
            var result = await _gradingCriteriaService.SetGradingCriteriaAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على جميع معايير التقدير (مع تصفية اختيارية)
        /// </summary>
        [HttpPost("get-criteria")]
        public async Task<ActionResult<GetGradingCriteriaResponse>> GetGradingCriteria(GetGradingCriteriaRequest request)
        {
            var result = await _gradingCriteriaService.GetGradingCriteriaAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// حذف معيار تقدير
        /// </summary>
        [HttpPost("delete-criteria")]
        public async Task<ActionResult<DeleteGradingCriteriaResponse>> DeleteGradingCriteria(DeleteGradingCriteriaRequest request)
        {
            var result = await _gradingCriteriaService.DeleteGradingCriteriaAsync(request);
            return Ok(result);
        }
    }
}
