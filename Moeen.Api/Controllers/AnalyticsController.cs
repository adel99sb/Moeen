using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Analytics;
using Moeen.Api.Shared.Responses.Analytics;
using System.Threading.Tasks;

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
        {
            var result = await _analyticsService.AnalyzeStudentDataAsync(request);
            return Ok(result);
        }

        [HttpPost("teacher")]
        public async Task<ActionResult<TeacherAnalyticsDto>> AnalyzeTeacher(AnalyzeTeacherPerformanceRequest request)
        {
            var result = await _analyticsService.AnalyzeTeacherPerformanceAsync(request);
            return Ok(result);
        }

        [HttpPost("circle")]
        public async Task<ActionResult<CircleAnalyticsDto>> AnalyzeCircle(AnalyzeCircleEffectivenessRequest request)
        {
            var result = await _analyticsService.AnalyzeCircleEffectivenessAsync(request);
            return Ok(result);
        }
    }
}