using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Review;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Review;

namespace Moeen.Api.Controllers
{
    [Route("api/review-management")]
    [ApiController]
    public class ReviewManagementController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewManagementController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost("record-page")]
        public async Task<ActionResult<RecordReviewPageResponse>> RecordReviewPage([FromBody] RecordReviewPageRequest request)
            => Ok(await _reviewService.RecordReviewPageAsync(request));

        [HttpPost("record-juz")]
        public async Task<ActionResult<ReviewResultDto>> RecordJuzReview([FromBody] RecordJuzReviewRequest request)
            => Ok(await _reviewService.RecordJuzReviewAsync(request));

        [HttpPost("record-multiple-juz")]
        public async Task<ActionResult<MultipleJuzReviewResponse>> RecordMultipleJuzReview([FromBody] RecordMultipleJuzReviewRequest request)
            => Ok(await _reviewService.RecordMultipleJuzReviewAsync(request));

        [HttpGet("records/{reviewRecordId:guid}")]
        public async Task<ActionResult<ReviewRecordDto>> GetReviewRecordById([FromRoute] Guid reviewRecordId)
            => Ok(await _reviewService.GetReviewRecordByIdAsync(new GetReviewRecordByIdRequest { ReviewRecordId = reviewRecordId }));

        [HttpGet("students/{studentId:guid}/history")]
        public async Task<ActionResult<PagedList<ReviewRecordDto>>> GetStudentReviewHistory(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
            => Ok(await _reviewService.GetStudentReviewHistoryAsync(new GetStudentReviewHistoryRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));

        [HttpGet("students/{studentId:guid}/by-date")]
        public async Task<ActionResult<List<ReviewRecordDto>>> GetReviewsByDateRange(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
            => Ok(await _reviewService.GetReviewsByDateRangeAsync(new GetReviewsByDateRangeRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        [HttpGet("circles/{circleId:guid}/progress")]
        public async Task<ActionResult<List<StudentReviewSummaryDto>>> GetCircleReviewProgress([FromRoute] Guid circleId)
            => Ok(await _reviewService.GetCircleReviewProgressAsync(new GetCircleReviewProgressRequest { CircleId = circleId }));

        [HttpGet("teachers/{teacherId:guid}")]
        public async Task<ActionResult<List<TeacherReviewSummaryDto>>> GetReviewsByTeacher(
            [FromRoute] Guid teacherId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
            => Ok(await _reviewService.GetReviewsByTeacherAsync(new GetReviewsByTeacherRequest
            {
                TeacherId = teacherId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        [HttpPut("records/{reviewRecordId:guid}/grade")]
        public async Task<ActionResult<ReviewRecordDto>> UpdateReviewGrade(
            [FromRoute] Guid reviewRecordId,
            [FromBody] UpdateReviewGradeRequest request)
        {
            request.ReviewRecordId = reviewRecordId;
            return Ok(await _reviewService.UpdateReviewGradeAsync(request));
        }

        [HttpDelete("records/{reviewRecordId:guid}")]
        public async Task<ActionResult<ReviewOperationResponse>> DeleteReviewRecord([FromRoute] Guid reviewRecordId)
            => Ok(await _reviewService.DeleteReviewRecordAsync(new DeleteReviewRecordRequest { ReviewRecordId = reviewRecordId }));

        [HttpPost("certificates")]
        public async Task<ActionResult<ReviewCertificateDto>> GenerateReviewCertificate([FromBody] GenerateReviewCertificateRequest request)
            => Ok(await _reviewService.GenerateReviewCertificateAsync(request));
    }
}