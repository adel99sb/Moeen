using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.ExamCommand;
using Moeen.Shared.Responses.ExamCommand;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamCommandController : ControllerBase
    {
        private readonly IExamCommandService _examCommandService;

        public ExamCommandController(IExamCommandService examCommandService)
        {
            _examCommandService = examCommandService;
        }

        /// <summary>
        /// أمر: تسجيل اختبار جديد.
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<ExamResultDto>> RegisterExam([FromBody] RegisterExamRequest request)
            => Ok(await _examCommandService.RegisterExamAsync(request));

        /// <summary>
        /// أمر: تحديث نتيجة اختبار.
        /// </summary>
        [HttpPut("update-result")]
        public async Task<ActionResult<ExamResultDto>> UpdateExamResult([FromBody] UpdateExamResultRequest request)
            => Ok(await _examCommandService.UpdateExamResultAsync(request));

        /// <summary>
        /// أمر: تحديث البيانات الوصفية للاختبار (التاريخ، النوع، الملاحظات).
        /// </summary>
        [HttpPut("update-info")]
        public async Task<ActionResult<ExamResultDto>> UpdateExamInfo([FromBody] UpdateExamInfoRequest request)
            => Ok(await _examCommandService.UpdateExamInfoAsync(request));

        /// <summary>
        /// أمر: إضافة ملاحظات تقييمية وتوصيات على الاختبار.
        /// </summary>
        [HttpPut("add-feedback")]
        public async Task<ActionResult<ExamFeedbackDto>> AddExamFeedback([FromBody] AddExamFeedbackRequest request)
            => Ok(await _examCommandService.AddExamFeedbackAsync(request));

        /// <summary>
        /// أمر: حذف نتيجة اختبار.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<ActionResult<DeleteExamResultResponse>> DeleteExamResult([FromBody] DeleteExamResultRequest request)
            => Ok(await _examCommandService.DeleteExamResultAsync(request));
    }
}