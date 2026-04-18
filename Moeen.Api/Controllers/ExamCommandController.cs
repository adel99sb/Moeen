using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamCommand;
using Moeen.Api.Shared.Responses.ExamCommand;

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
        public async Task<ActionResult<ExamResultDto>> RegisterExam(RegisterExamRequest request)
            => Ok(await _examCommandService.RegisterExamAsync(request));

        /// <summary>
        /// أمر: تحديث نتيجة اختبار.
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<ExamResultDto>> UpdateExamResult(UpdateExamResultRequest request)
            => Ok(await _examCommandService.UpdateExamResultAsync(request));

        /// <summary>
        /// أمر: حذف نتيجة اختبار.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<ActionResult<DeleteExamResultResponse>> DeleteExamResult(DeleteExamResultRequest request)
            => Ok(await _examCommandService.DeleteExamResultAsync(request));
    }
}