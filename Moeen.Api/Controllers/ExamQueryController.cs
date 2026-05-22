using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.ExamQuery;
using Moeen.Shared.Responses;
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
        public async Task<ActionResult<GeneralResponse>> GetExamResultById([FromBody] GetExamResultByIdRequest request)
        {
            var r = await _examQueryService.GetExamResultByIdAsync(request);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// GET (جديد): جلب نتيجة اختبار مباشرة.
        /// </summary>
        [HttpGet("{examId:guid}")]
        public async Task<ActionResult<GeneralResponse>> GetExamResultByIdGet([FromRoute] Guid examId)
        {
            var r = await _examQueryService.GetExamResultByIdAsync(new GetExamResultByIdRequest { ExamId = examId });
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// البحث في نتائج الاختبارات
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<GeneralResponse>> SearchExamResults([FromBody] SearchExamResultsRequest request)
        {
            var r = await _examQueryService.SearchExamResultsAsync(request);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// الحصول على اختبارات طالب معين
        /// </summary>
        [HttpPost("student-exams")]
        public async Task<ActionResult<GeneralResponse>> GetStudentExams([FromBody] GetStudentExamsRequest request)
        {
            var r = await _examQueryService.GetStudentExamsAsync(request);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// GET (جديد): جلب اختبارات الطالب.
        /// </summary>
        [HttpGet("students/{studentId:guid}/exams")]
        public async Task<ActionResult<GeneralResponse>> GetStudentExamsGet([FromRoute] Guid studentId)
        {
            var r = await _examQueryService.GetStudentExamsAsync(new GetStudentExamsRequest { StudentId = studentId });
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// GET: نتائج اختبارات حلقة مع التصفح.
        /// </summary>
        [HttpGet("halqas/{halqaId:guid}/exams")]
        public async Task<ActionResult<GeneralResponse>> GetExamsByHalqa([FromRoute] Guid halqaId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var r = await _examQueryService.GetExamsByHalqaAsync(new GetExamsByHalqaRequest { HalqaId = halqaId, FromDate = fromDate, ToDate = toDate, PageNumber = pageNumber, PageSize = pageSize });
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// GET: اختبارات طالب ضمن فترة.
        /// </summary>
        [HttpGet("students/{studentId:guid}/exams/by-date")]
        public async Task<ActionResult<GeneralResponse>> GetStudentExamsByDateRange([FromRoute] Guid studentId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var r = await _examQueryService.GetStudentExamsByDateRangeAsync(new GetStudentExamsByDateRequest { StudentId = studentId, FromDate = fromDate, ToDate = toDate });
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// GET: اختبارات معلم.
        /// </summary>
        [HttpGet("teachers/{teacherId:guid}/exams")]
        public async Task<ActionResult<GeneralResponse>> GetExamsByTeacher([FromRoute] Guid teacherId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var r = await _examQueryService.GetExamsByTeacherAsync(new GetExamsByTeacherRequest { TeacherId = teacherId, FromDate = fromDate, ToDate = toDate });
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// GET: اختبارات حسب المرحلة.
        /// </summary>
        //[HttpGet("phases/{phaseId:guid}/exams")]
        //public async Task<ActionResult<GeneralResponse>> GetExamsByPhase([FromRoute] Guid phaseId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        //{
        //    var r = await _examQueryService.GetExamsByPhaseAsync(new GetExamsByPhaseRequest { PhaseId = phaseId, FromDate = fromDate, ToDate = toDate });
        //    return StatusCode(r.StatusCode, r);
        //}

        /// <summary>
        /// GET: إحصائيات شاملة للاختبارات.
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<GeneralResponse>> GetExamStatistics([FromQuery] GetExamStatisticsRequest request)
        {
            var r = await _examQueryService.GetExamStatisticsAsync(request);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// GET: تحليلات اختبارات حلقة.
        /// </summary>
        [HttpGet("Halqa/{HalqaId:guid}/analytics")]
        public async Task<ActionResult<GeneralResponse>> GetHalqaExamAnalytics([FromRoute] Guid HalqaId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var r = await _examQueryService.GetHalqaExamAnalyticsAsync(new GetHalqaAnalyticsRequest { HalqaId = HalqaId, FromDate = fromDate, ToDate = toDate });
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// POST: مقارنة أداء الحلقات.
        /// </summary>
        [HttpPost("compare-Halqa")]
        public async Task<ActionResult<GeneralResponse>> CompareHalqasPerformanceAsync([FromBody] CompareHalqasRequest request)
        {
            var r = await _examQueryService.CompareHalqasPerformanceAsync(request);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// POST: تجهيز بيانات الاختبارات للتصدير.
        /// </summary>
        [HttpPost("prepare-export")]
        public async Task<ActionResult<GeneralResponse>> PrepareExamDataForExport([FromBody] PrepareExportRequest request)
        {
            var r = await _examQueryService.PrepareExamDataForExportAsync(request);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// GET: جلب الطلاب الأكثر تميزاً في المختبر
        /// </summary>
        [HttpGet("lab/top-performers")]
        public async Task<ActionResult<GeneralResponse>> GetTopPerformingStudents([FromQuery] GetTopPerformingStudentsInExamsRequest request)
        {
            var response = await _examQueryService.GetTopPerformingStudentsInExamsAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// GET: جلب الطلاب الأقل تميزاً في المختبر
        /// </summary>
        [HttpGet("lab/lowest-performers")]
        public async Task<ActionResult<GeneralResponse>> GetLowestPerformingStudents([FromQuery] GetLowestPerformingStudentsInExamsRequest request)
        {
            var response = await _examQueryService.GetLowestPerformingStudentsInExamsAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// GET: إحصائيات عامة للمختبر
        /// </summary>
        [HttpGet("lab/statistics")]
        public async Task<ActionResult<GeneralResponse>> GetLabStatistics([FromQuery] GetLabStatisticsRequest request)
        {
            var response = await _examQueryService.GetLabStatisticsAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}