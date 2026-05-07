using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Analytics;
using Moeen.Shared.Responses.Analytics;

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
        public async Task<IActionResult> AnalyzeStudent([FromBody] AnalyzeStudentDataRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _analyticsService.AnalyzeStudentDataAsync(request);
            return result.ToActionResult();
        }

        /// <summary>
        /// GET (جديد): تحليل مباشر لبيانات طالب عبر المعرف.
        /// </summary>
        [HttpGet("student/{studentId:guid}")]
        public async Task<IActionResult> AnalyzeStudentGet([FromRoute] Guid studentId)
        {
            var result = await _analyticsService.AnalyzeStudentDataByIdAsync(studentId);
            return result.ToActionResult();
        }

        /// <summary>
        /// POST (قديم/متوافق): تحليل أداء معلم عبر Body.
        /// </summary>
        [HttpPost("teacher")]
        public async Task<IActionResult> AnalyzeTeacher([FromBody] AnalyzeTeacherPerformanceRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _analyticsService.AnalyzeTeacherPerformanceAsync(request);
            return result.ToActionResult();
        }

        /// <summary>
        /// GET (جديد): تحليل مباشر لأداء معلم عبر المعرف.
        /// </summary>
        [HttpGet("teacher/{teacherId:guid}")]
        public async Task<IActionResult> AnalyzeTeacherGet([FromRoute] Guid teacherId)
        {
            var result = await _analyticsService.AnalyzeTeacherPerformanceByIdAsync(teacherId);
            return result.ToActionResult();
        }

        /// <summary>
        /// POST (قديم/متوافق): تحليل فعالية حلقة عبر Body.
        /// </summary>
        [HttpPost("circle")]
        public async Task<IActionResult> AnalyzeCircle([FromBody] AnalyzeCircleEffectivenessRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _analyticsService.AnalyzeCircleEffectivenessAsync(request);
            return result.ToActionResult();
        }

        /// <summary>
        /// GET (جديد): تحليل مباشر لفعالية حلقة عبر المعرف.
        /// </summary>
        [HttpGet("circle/{circleId:guid}")]
        public async Task<IActionResult> AnalyzeCircleGet([FromRoute] Guid circleId)
        {
            var result = await _analyticsService.AnalyzeCircleEffectivenessByIdAsync(circleId);
            return result.ToActionResult();
        }

        /// <summary>
        /// POST: حفظ تقرير تحليلي.
        /// </summary>
        [HttpPost("reports")]
        public async Task<IActionResult> SaveReport([FromBody] SaveAnalyticsReportRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _analyticsService.SaveAnalyticsReportAsync(request);
            return result.ToActionResult();
        }

        /// <summary>
        /// GET: جلب تقرير تحليلي محفوظ عبر المعرف.
        /// </summary>
        [HttpGet("reports/{reportId:guid}")]
        public async Task<IActionResult> GetReportById([FromRoute] Guid reportId)
        {
            var result = await _analyticsService.GetAnalyticsReportByIdAsync(reportId);
            return result.ToActionResult();
        }

        /// <summary>
        /// GET: جلب التقارير التحليلية المحفوظة مع التصفية والتصفح.
        /// </summary>
        [HttpGet("reports")]
        public async Task<IActionResult> GetReports([FromQuery] AnalyticsReportFilter filter)
        {
            var result = await _analyticsService.GetAllAnalyticsReportsAsync(filter);
            return result.ToActionResult();
        }

        /// <summary>
        /// PUT: تحديث تقرير تحليلي محفوظ.
        /// </summary>
        [HttpPut("reports/{reportId:guid}")]
        public async Task<IActionResult> UpdateReport([FromRoute] Guid reportId, [FromBody] UpdateAnalyticsReportRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _analyticsService.UpdateAnalyticsReportAsync(reportId, request);
            return result.ToActionResult();
        }

        /// <summary>
        /// DELETE: حذف تقرير تحليلي محفوظ.
        /// </summary>
        [HttpDelete("reports/{reportId:guid}")]
        public async Task<IActionResult> DeleteReport([FromRoute] Guid reportId)
        {
            var result = await _analyticsService.DeleteAnalyticsReportAsync(reportId);
            return result.ToActionResult();
        }
    }
}