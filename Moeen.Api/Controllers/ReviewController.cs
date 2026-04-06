using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Review;
using Moeen.Api.Shared.Responses.Review;
using System.Threading.Tasks;

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
        /// تسجيل مراجعة صفحة
        /// </summary>
        [HttpPost("record-page")]
        public async Task<ActionResult<RecordReviewPageResponse>> RecordReviewPage(RecordReviewPageRequest request)
        {
            var result = await _reviewService.RecordReviewPageAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تسجيل مراجعة جزء
        /// </summary>
        [HttpPost("record-juz")]
        public async Task<ActionResult<ReviewResultDto>> RecordJuzReview(RecordJuzReviewRequest request)
        {
            var result = await _reviewService.RecordJuzReviewAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تسجيل مراجعة عدة أجزاء دفعة واحدة
        /// </summary>
        [HttpPost("record-multiple-juz")]
        public async Task<ActionResult<MultipleJuzReviewResponse>> RecordMultipleJuzReview(RecordMultipleJuzReviewRequest request)
        {
            var result = await _reviewService.RecordMultipleJuzReviewAsync(request);
            return Ok(result);
        }
    }
}