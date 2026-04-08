using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamQuery;
using Moeen.Api.Shared.Responses.ExamCommand;
using Moeen.Api.Shared.Responses.ExamQuery;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamQueryController : ControllerBase
    {
        private readonly IExamQueryService _examQueryService;

        public ExamQueryController(IExamQueryService examQueryService)
        {
            _examQueryService = examQueryService;
        }

        /// <summary>
        /// الحصول على نتيجة اختبار بواسطة المعرف
        /// </summary>
        [HttpPost("get-by-id")]
        public async Task<ActionResult<ExamResultDto>> GetExamResultById(GetExamResultByIdRequest request)
        {
            var result = await _examQueryService.GetExamResultByIdAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// البحث في نتائج الاختبارات
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<SearchExamResultsResponse>> SearchExamResults(SearchExamResultsRequest request)
        {
            var result = await _examQueryService.SearchExamResultsAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على اختبارات طالب معين
        /// </summary>
        [HttpPost("student-exams")]
        public async Task<ActionResult<GetStudentExamsResponse>> GetStudentExams(GetStudentExamsRequest request)
        {
            var result = await _examQueryService.GetStudentExamsAsync(request);
            return Ok(result);
        }
    }
}