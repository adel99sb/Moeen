using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.ExamQuery;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.ExamCommand;
using Moeen.Shared.Responses.ExamQuery;

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

        /// <summary>
        /// GET: نتائج اختبارات حلقة مع التصفح.
        /// </summary>
        [HttpGet("circles/{circleId:guid}/exams")]
        public async Task<ActionResult<PagedList<ExamResultDto>>> GetExamsByCircle(
            [FromRoute] Guid circleId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
            => Ok(await _examQueryService.GetExamsByCircleAsync(new GetExamsByCircleRequest
            {
                CircleId = circleId,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));

        /// <summary>
        /// GET: اختبارات طالب ضمن فترة.
        /// </summary>
        [HttpGet("students/{studentId:guid}/exams/by-date")]
        public async Task<ActionResult<List<ExamResultDto>>> GetStudentExamsByDateRange(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
            => Ok(await _examQueryService.GetStudentExamsByDateRangeAsync(new GetStudentExamsByDateRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        /// <summary>
        /// GET: اختبارات معلم.
        /// </summary>
        [HttpGet("teachers/{teacherId:guid}/exams")]
        public async Task<ActionResult<List<ExamResultDto>>> GetExamsByTeacher(
            [FromRoute] Guid teacherId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
            => Ok(await _examQueryService.GetExamsByTeacherAsync(new GetExamsByTeacherRequest
            {
                TeacherId = teacherId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        /// <summary>
        /// GET: اختبارات حسب المرحلة.
        /// </summary>
        [HttpGet("phases/{phaseId:guid}/exams")]
        public async Task<ActionResult<List<ExamResultDto>>> GetExamsByPhase(
            [FromRoute] Guid phaseId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
            => Ok(await _examQueryService.GetExamsByPhaseAsync(new GetExamsByPhaseRequest
            {
                PhaseId = phaseId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        /// <summary>
        /// GET: إحصائيات شاملة للاختبارات.
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<ExamStatisticsDto>> GetExamStatistics([FromQuery] GetExamStatisticsRequest request)
            => Ok(await _examQueryService.GetExamStatisticsAsync(request));

        /// <summary>
        /// GET: تحليلات اختبارات حلقة.
        /// </summary>
        [HttpGet("circles/{circleId:guid}/analytics")]
        public async Task<ActionResult<CircleExamAnalyticsDto>> GetCircleExamAnalytics(
            [FromRoute] Guid circleId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
            => Ok(await _examQueryService.GetCircleExamAnalyticsAsync(new GetCircleAnalyticsRequest
            {
                CircleId = circleId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        /// <summary>
        /// POST: مقارنة أداء الحلقات.
        /// </summary>
        [HttpPost("compare-circles")]
        public async Task<ActionResult<CircleComparisonDto>> CompareCirclesPerformance([FromBody] CompareCirclesRequest request)
            => Ok(await _examQueryService.CompareCirclesPerformanceAsync(request));

        /// <summary>
        /// POST: تجهيز بيانات الاختبارات للتصدير.
        /// </summary>
        [HttpPost("prepare-export")]
        public async Task<ActionResult<ExportExamDataDto>> PrepareExamDataForExport([FromBody] PrepareExportRequest request)
            => Ok(await _examQueryService.PrepareExamDataForExportAsync(request));
    }
}