using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Feedback;
using Moeen.Api.Shared.Responses.Feedback;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        /// <summary>
        /// أمر: تقديم شكوى.
        /// </summary>
        [HttpPost("complaint")]
        public async Task<ActionResult<SubmitComplaintResponse>> SubmitComplaint(SubmitComplaintRequest request)
            => Ok(await _feedbackService.SubmitComplaintAsync(request));

        /// <summary>
        /// أمر: تقديم اقتراح.
        /// </summary>
        [HttpPost("suggestion")]
        public async Task<ActionResult<SubmitSuggestionResponse>> SubmitSuggestion(SubmitSuggestionRequest request)
            => Ok(await _feedbackService.SubmitSuggestionAsync(request));

        /// <summary>
        /// أمر: إدارة الشكاوى والاقتراحات.
        /// </summary>
        [HttpPut("manage")]
        public async Task<ActionResult<ManageFeedbackResponse>> ManageFeedback(ManageFeedbackRequest request)
            => Ok(await _feedbackService.ManageFeedbacksAsync(request));
    }
}