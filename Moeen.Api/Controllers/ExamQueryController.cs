using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamQuery;
using Moeen.Api.Shared.Responses.ExamCommand;
using Moeen.Api.Shared.Responses.ExamQuery;
using System;

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
            => Ok(await _examQueryService.GetExamResultByIdAsync(request));

        /// <summary>
        /// GET (جديد): جلب نتيجة اختبار مباشرة.
        /// </summary>
        [HttpGet("{examId:guid}")]
        public async Task<ActionResult<ExamResultDto>> GetExamResultByIdGet([FromRoute] Guid examId)
            => Ok(await _examQueryService.GetExamResultByIdAsync(new GetExamResultByIdRequest { ExamId = examId }));

        /// <summary>
        /// البحث في نتائج الاختبارات
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<SearchExamResultsResponse>> SearchExamResults(SearchExamResultsRequest request)
            => Ok(await _examQueryService.SearchExamResultsAsync(request));

        /// <summary>
        /// الحصول على اختبارات طالب معين
        /// </summary>
        [HttpPost("student-exams")]
        public async Task<ActionResult<GetStudentExamsResponse>> GetStudentExams(GetStudentExamsRequest request)
            => Ok(await _examQueryService.GetStudentExamsAsync(request));

        /// <summary>
        /// GET (جديد): جلب اختبارات الطالب.
        /// </summary>
        [HttpGet("students/{studentId:guid}/exams")]
        public async Task<ActionResult<GetStudentExamsResponse>> GetStudentExamsGet([FromRoute] Guid studentId)
            => Ok(await _examQueryService.GetStudentExamsAsync(new GetStudentExamsRequest { StudentId = studentId }));
       }
}