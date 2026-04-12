using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Review;
using Moeen.Api.Shared.Responses.Review;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// أمر: تسجيل مراجعة صفحة.
        /// </summary>
        [HttpPost("record-page")]
        public async Task<ActionResult<RecordReviewPageResponse>> RecordReviewPage(RecordReviewPageRequest request)
            => Ok(await _reviewService.RecordReviewPageAsync(request));

        /// <summary>
        /// أمر: تسجيل مراجعة جزء.
        /// </summary>
        [HttpPost("record-juz")]
        public async Task<ActionResult<ReviewResultDto>> RecordJuzReview(RecordJuzReviewRequest request)
            => Ok(await _reviewService.RecordJuzReviewAsync(request));

        /// <summary>
        /// أمر: تسجيل مراجعة عدة أجزاء.
        /// </summary>
        [HttpPost("record-multiple-juz")]
        public async Task<ActionResult<MultipleJuzReviewResponse>> RecordMultipleJuzReview(RecordMultipleJuzReviewRequest request)
            => Ok(await _reviewService.RecordMultipleJuzReviewAsync(request));
    }
}