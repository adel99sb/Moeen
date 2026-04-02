using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamCommand;
using Moeen.Api.Shared.Responses.ExamCommand;
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
        /// تسجيل اختبار جديد
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<ExamResultDto>> RegisterExam(RegisterExamRequest request)
        {
            var result = await _examCommandService.RegisterExamAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تحديث نتيجة اختبار
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<ExamResultDto>> UpdateExamResult(UpdateExamResultRequest request)
        {
            var result = await _examCommandService.UpdateExamResultAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// حذف نتيجة اختبار
        /// </summary>
        [HttpDelete("delete")]
        public async Task<ActionResult<DeleteExamResultResponse>> DeleteExamResult(DeleteExamResultRequest request)
        {
            var result = await _examCommandService.DeleteExamResultAsync(request);
            return Ok(result);
        }
    }
}