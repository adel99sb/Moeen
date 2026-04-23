using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Analytics;
using Moeen.Api.Shared.Responses.Analytics;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>
        /// POST (قديم/متوافق): تحليل بيانات طالب عبر Body.
        /// </summary>
        [HttpPost("student")]
        public async Task<ActionResult<StudentAnalyticsDto>> AnalyzeStudent(AnalyzeStudentDataRequest request)
            => Ok(await _analyticsService.AnalyzeStudentDataAsync(request));

        /// <summary>
        /// GET (جديد): تحليل مباشر لبيانات طالب عبر المعرف.
        /// </summary>
        [HttpGet("student/{studentId:guid}")]
        public async Task<ActionResult<StudentAnalyticsDto>> AnalyzeStudentGet([FromRoute] Guid studentId)
            => Ok(await _analyticsService.AnalyzeStudentDataByIdAsync(studentId));

        /// <summary>
        /// POST (قديم/متوافق): تحليل أداء معلم عبر Body.
        /// </summary>
        [HttpPost("teacher")]
        public async Task<ActionResult<TeacherAnalyticsDto>> AnalyzeTeacher(AnalyzeTeacherPerformanceRequest request)
            => Ok(await _analyticsService.AnalyzeTeacherPerformanceAsync(request));

        /// <summary>
        /// GET (جديد): تحليل مباشر لأداء معلم عبر المعرف.
        /// </summary>
        [HttpGet("teacher/{teacherId:guid}")]
        public async Task<ActionResult<TeacherAnalyticsDto>> AnalyzeTeacherGet([FromRoute] Guid teacherId)
            => Ok(await _analyticsService.AnalyzeTeacherPerformanceByIdAsync(teacherId));

        /// <summary>
        /// POST (قديم/متوافق): تحليل فعالية حلقة عبر Body.
        /// </summary>
        [HttpPost("circle")]
        public async Task<ActionResult<CircleAnalyticsDto>> AnalyzeCircle(AnalyzeCircleEffectivenessRequest request)
            => Ok(await _analyticsService.AnalyzeCircleEffectivenessAsync(request));

        /// <summary>
        /// GET (جديد): تحليل مباشر لفعالية حلقة عبر المعرف.
        /// </summary>
        [HttpGet("circle/{circleId:guid}")]
        public async Task<ActionResult<CircleAnalyticsDto>> AnalyzeCircleGet([FromRoute] Guid circleId)
            => Ok(await _analyticsService.AnalyzeCircleEffectivenessByIdAsync(circleId));

        /// <summary>
        /// POST: حفظ تقرير تحليلي.
        /// </summary>
        [HttpPost("reports")]
        public async Task<ActionResult<AnalyticsReportDto>> SaveReport([FromBody] SaveAnalyticsReportRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _analyticsService.SaveAnalyticsReportAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// GET: جلب تقرير تحليلي محفوظ عبر المعرف.
        /// </summary>
        [HttpGet("reports/{reportId:guid}")]
        public async Task<ActionResult<AnalyticsReportDto>> GetReportById([FromRoute] Guid reportId)
            => Ok(await _analyticsService.GetAnalyticsReportByIdAsync(reportId));

        /// <summary>
        /// GET: جلب التقارير التحليلية المحفوظة مع التصفية والتصفح.
        /// </summary>
        [HttpGet("reports")]
        public async Task<ActionResult<PagedResult<AnalyticsReportSummaryDto>>> GetReports([FromQuery] AnalyticsReportFilter filter)
            => Ok(await _analyticsService.GetAllAnalyticsReportsAsync(filter));

        /// <summary>
        /// PUT: تحديث تقرير تحليلي محفوظ.
        /// </summary>
        [HttpPut("reports/{reportId:guid}")]
        public async Task<ActionResult<AnalyticsReportDto>> UpdateReport(
            [FromRoute] Guid reportId,
            [FromBody] UpdateAnalyticsReportRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _analyticsService.UpdateAnalyticsReportAsync(reportId, request));
        }

        /// <summary>
        /// DELETE: حذف تقرير تحليلي محفوظ.
        /// </summary>
        [HttpDelete("reports/{reportId:guid}")]
        public async Task<ActionResult<bool>> DeleteReport([FromRoute] Guid reportId)
            => Ok(await _analyticsService.DeleteAnalyticsReportAsync(reportId));
    }
}