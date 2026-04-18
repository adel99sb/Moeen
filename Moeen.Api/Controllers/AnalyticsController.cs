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

        [HttpPost("student")]
        public async Task<ActionResult<StudentAnalyticsDto>> AnalyzeStudent(AnalyzeStudentDataRequest request)
            => Ok(await _analyticsService.AnalyzeStudentDataAsync(request));

        /// <summary>
        /// GET (جديد): تحليل طالب عبر Route.
        /// </summary>
        [HttpGet("student/{studentId:guid}")]
        public async Task<ActionResult<StudentAnalyticsDto>> AnalyzeStudentGet([FromRoute] Guid studentId)
            => Ok(await _analyticsService.AnalyzeStudentDataAsync(new AnalyzeStudentDataRequest { Id = studentId }));

        [HttpPost("teacher")]
        public async Task<ActionResult<TeacherAnalyticsDto>> AnalyzeTeacher(AnalyzeTeacherPerformanceRequest request)
            => Ok(await _analyticsService.AnalyzeTeacherPerformanceAsync(request));

        /// <summary>
        /// GET (جديد): تحليل معلم عبر Route.
        /// </summary>
        [HttpGet("teacher/{teacherId:guid}")]
        public async Task<ActionResult<TeacherAnalyticsDto>> AnalyzeTeacherGet([FromRoute] Guid teacherId)
            => Ok(await _analyticsService.AnalyzeTeacherPerformanceAsync(new AnalyzeTeacherPerformanceRequest { Id = teacherId }));

        [HttpPost("circle")]
        public async Task<ActionResult<CircleAnalyticsDto>> AnalyzeCircle(AnalyzeCircleEffectivenessRequest request)
            => Ok(await _analyticsService.AnalyzeCircleEffectivenessAsync(request));

        /// <summary>
        /// GET (جديد): تحليل حلقة عبر Route.
        /// </summary>
        [HttpGet("circle/{circleId:guid}")]
        public async Task<ActionResult<CircleAnalyticsDto>> AnalyzeCircleGet([FromRoute] Guid circleId)
            => Ok(await _analyticsService.AnalyzeCircleEffectivenessAsync(new AnalyzeCircleEffectivenessRequest { Id = circleId }));
    }
}