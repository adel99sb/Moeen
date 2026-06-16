using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.LessonManagement;
using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/lesson-management")]
    [ApiController]
    public class LessonManagementController : ControllerBase
    {
        private readonly ILessonManagementService _lessonService;

        public LessonManagementController(ILessonManagementService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpPost("lessons")]
        public async Task<ActionResult<GeneralResponse>> CreateLesson([FromBody] CreateLessonRequest request)
            => Ok(await _lessonService.CreateLessonAsync(request));

        [HttpPut("lessons/{lessonId:guid}")]
        public async Task<ActionResult<GeneralResponse>> UpdateLesson([FromRoute] Guid lessonId, [FromBody] UpdateLessonRequest request)
        {
            request.Id = lessonId;
            return Ok(await _lessonService.UpdateLessonAsync(request));
        }

        [HttpDelete("lessons/{lessonId:guid}")]
        public async Task<ActionResult<GeneralResponse>> DeleteLesson([FromRoute] Guid lessonId)
            => Ok(await _lessonService.DeleteLessonAsync(new DeleteLessonRequest { Id = lessonId }));

        [HttpGet("circles/{circleId:guid}/lessons")]
        public async Task<ActionResult<GeneralResponse>> GetLessonsByCircle([FromRoute] Guid circleId)
            => Ok(await _lessonService.GetLessonsByCircleAsync(new GetLessonsByCircleRequest { CircleId = circleId }));

        [HttpGet("dashboard/weekly")]
        public async Task<ActionResult<GeneralResponse>> GetWeeklyDashboard([FromQuery] DateTime? date)
            => Ok(await _lessonService.GetWeeklyLessonDashboardAsync(new GetWeeklyLessonDashboardRequest { Date = date }));

        [HttpPost("attendance")]
        public async Task<ActionResult<GeneralResponse>> RecordLessonAttendance([FromBody] RecordAttendanceRequest request)
            => Ok(await _lessonService.RecordLessonAttendanceAsync(request));

        [HttpGet("circles/{circleId:guid}/history")]
        public async Task<ActionResult<GeneralResponse>> GetLessonHistory(
            [FromRoute] Guid circleId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
            => Ok(await _lessonService.GetLessonHistoryAsync(new GetLessonHistoryRequest
            {
                CircleId = circleId,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));

        [HttpGet("circles/overview")]
        public async Task<ActionResult<GeneralResponse>> GetCirclesOverview([FromQuery] Guid? mosqueId)
            => Ok(await _lessonService.GetCirclesOverviewAsync(new GetCirclesOverviewRequest { MosqueId = mosqueId }));

        [HttpGet("weekly-lessons")]
        public async Task<ActionResult<GeneralResponse>> GetManagedWeeklyLessons()
            => Ok(await _lessonService.GetManagedWeeklyLessonsAsync());

        [HttpPost("weekly-lessons")]
        public async Task<ActionResult<GeneralResponse>> CreateWeeklyLesson([FromBody] CreateWeeklyLessonRequest request)
            => Ok(await _lessonService.CreateWeeklyLessonAsync(request));

        [HttpPut("weekly-lessons/{weeklyLessonId:guid}")]
        public async Task<ActionResult<GeneralResponse>> UpdateWeeklyLesson([FromRoute] Guid weeklyLessonId, [FromBody] UpdateWeeklyLessonRequest request)
        {
            request.Id = weeklyLessonId;
            return Ok(await _lessonService.UpdateWeeklyLessonAsync(request));
        }

        [HttpDelete("weekly-lessons/{weeklyLessonId:guid}")]
        public async Task<ActionResult<GeneralResponse>> DeleteWeeklyLesson([FromRoute] Guid weeklyLessonId)
            => Ok(await _lessonService.DeleteWeeklyLessonAsync(weeklyLessonId));

        [HttpPost("weekly-lessons/{weeklyLessonId:guid}/rows")]
        public async Task<ActionResult<GeneralResponse>> CreateWeeklyLessonRow([FromRoute] Guid weeklyLessonId, [FromBody] CreateWeeklyLessonAssignmentRequest request)
        {
            request.WeeklyLessonId = weeklyLessonId;
            return Ok(await _lessonService.CreateWeeklyLessonAssignmentAsync(request));
        }

        [HttpPut("weekly-lesson-rows/{rowId:guid}")]
        public async Task<ActionResult<GeneralResponse>> UpdateWeeklyLessonRow([FromRoute] Guid rowId, [FromBody] UpdateWeeklyLessonAssignmentRequest request)
        {
            request.Id = rowId;
            return Ok(await _lessonService.UpdateWeeklyLessonAssignmentAsync(request));
        }

        [HttpDelete("weekly-lesson-rows/{rowId:guid}")]
        public async Task<ActionResult<GeneralResponse>> DeleteWeeklyLessonRow([FromRoute] Guid rowId)
            => Ok(await _lessonService.DeleteWeeklyLessonAssignmentAsync(rowId));

        [HttpGet("students/{studentId:guid}/daily-lessons")]
        public async Task<ActionResult<GeneralResponse>> GetStudentDailyLessons(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime date)
            => Ok(await _lessonService.GetStudentDailyLessonsAsync(new GetStudentDailyLessonsRequest
            {
                StudentId = studentId,
                Date = date
            }));
    }
}