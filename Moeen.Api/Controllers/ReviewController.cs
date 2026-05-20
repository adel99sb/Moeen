using Microsoft.AspNetCore.Authorization; // ✅ مطلوب لـ [Authorize]
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Review;
using Moeen.Shared.Responses;
using System;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // ✅ ضروري: لضمان أن CurrentUserId يعمل وتفعيل حماية الصلاحيات
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// تسجيل مراجعة يومية
        /// </summary>
        [HttpPost("record-page")]
        public async Task<ActionResult<GeneralResponse>> RecordReviewPage([FromBody] RecordReviewPageRequest request)
        {
            // ✅ اختياري: تحقق مبكر من صحة البيانات
            // if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _reviewService.RecordReviewPageAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// تسجيل مراجعة جزء
        /// </summary>
        [HttpPost("record-juz")]
        public async Task<ActionResult<GeneralResponse>> RecordJuzReview([FromBody] RecordJuzReviewRequest request)
        {
            var response = await _reviewService.RecordJuzReviewAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// جلب سجل مراجعة محدد
        /// </summary>
        [HttpGet("{reviewRecordId:guid}")]
        public async Task<ActionResult<GeneralResponse>> GetReviewRecordById([FromRoute] Guid reviewRecordId)
        {
            var response = await _reviewService.GetReviewRecordByIdAsync(new GetReviewRecordByIdRequest
            {
                ReviewRecordId = reviewRecordId
            });

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// جلب سجل مراجعات الطالب
        /// </summary>
        [HttpGet("students/{studentId:guid}/history")]
        public async Task<ActionResult<GeneralResponse>> GetStudentReviewHistory(
            [FromRoute] Guid studentId,
            [FromQuery] GetStudentReviewHistoryRequest request)
        {
            request ??= new GetStudentReviewHistoryRequest();
            request.StudentId = studentId;

            var response = await _reviewService.GetStudentReviewHistoryAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// تحديث تقييم مراجعة
        /// </summary>
        [HttpPut("update-grade")]
        public async Task<ActionResult<GeneralResponse>> UpdateReviewGrade([FromBody] UpdateReviewGradeRequest request)
        {
            var response = await _reviewService.UpdateReviewGradeAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// حذف سجل مراجعة
        /// </summary>
        [HttpDelete("{reviewRecordId:guid}")]
        public async Task<ActionResult<GeneralResponse>> DeleteReviewRecord([FromRoute] Guid reviewRecordId)
        {
            var response = await _reviewService.DeleteReviewRecordAsync(new DeleteReviewRecordRequest
            {
                ReviewRecordId = reviewRecordId
            });

            return StatusCode(response.StatusCode, response);
        }
    }
}