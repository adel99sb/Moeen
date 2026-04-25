using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamGrading;
using Moeen.Api.Shared.Responses.ExamGrading;
using System;

namespace Moeen.Api.Controllers
{
    /// <summary>
    /// التحكم في معايير تقدير الاختبارات
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExamGradingCriteriaController : ControllerBase
    {
        private readonly IExamGradingCriteriaService _examGradingCriteriaService;

        public ExamGradingCriteriaController(IExamGradingCriteriaService examGradingCriteriaService)
        {
            _examGradingCriteriaService = examGradingCriteriaService;
        }

        /// <summary>
        /// الحصول على جميع معايير التقدير (مع إمكانية التصفية حسب نوع الاختبار)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<GetGradingCriteriaResponse>> GetGradingCriteria([FromQuery] GetGradingCriteriaRequest request)
        {
            var result = await _examGradingCriteriaService.GetGradingCriteriaAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// جلب تفاصيل معيار تقدير محدد بواسطة معرفه
        /// </summary>
        [HttpGet("{criteriaId:guid}")]
        public async Task<ActionResult<GradingCriteriaDto>> GetCriteriaById([FromRoute] Guid criteriaId)
        {
            var request = new GetCriteriaByIdRequest { CriteriaId = criteriaId };
            var result = await _examGradingCriteriaService.GetCriteriaByIdAsync(request);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// إنشاء/تحديث معيار تقدير في نقطة واحدة
        /// </summary>
        [HttpPost("set-criteria")]
        public async Task<ActionResult<GradingCriteriaDto>> SetGradingCriteria([FromBody] SetGradingCriteriaRequest request)
        {
            var result = await _examGradingCriteriaService.SetGradingCriteriaAsync(request);

            // تحديث إذا كان Id موجود مسبقًا
            if (request.Id.HasValue && request.Id.Value != Guid.Empty)
                return Ok(result);

            return CreatedAtAction(nameof(GetCriteriaById), new { criteriaId = result.Id }, result);
        }

        /// <summary>
        /// حذف معيار تقدير (حذف نهائي)
        /// </summary>
        [HttpDelete("{criteriaId:guid}")]
        public async Task<ActionResult<DeleteGradingCriteriaResponse>> DeleteGradingCriteria([FromRoute] Guid criteriaId)
        {
            var request = new DeleteGradingCriteriaRequest { CriteriaId = criteriaId };
            var result = await _examGradingCriteriaService.DeleteGradingCriteriaAsync(request);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
