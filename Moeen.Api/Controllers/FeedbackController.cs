using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Feedback;
using Moeen.Api.Shared.Responses.Feedback;
using System.Threading.Tasks;

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
        /// تقديم شكوى
        /// </summary>
        [HttpPost("complaint")]
        public async Task<ActionResult<SubmitComplaintResponse>> SubmitComplaint(SubmitComplaintRequest request)
        {
            var result = await _feedbackService.SubmitComplaintAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تقديم اقتراح
        /// </summary>
        [HttpPost("suggestion")]
        public async Task<ActionResult<SubmitSuggestionResponse>> SubmitSuggestion(SubmitSuggestionRequest request)
        {
            var result = await _feedbackService.SubmitSuggestionAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// إدارة الشكاوى والاقتراحات (الرد)
        /// </summary>
        [HttpPut("manage")]
        public async Task<ActionResult<ManageFeedbackResponse>> ManageFeedback(ManageFeedbackRequest request)
        {
            var result = await _feedbackService.ManageFeedbacksAsync(request);
            return Ok(result);
        }
    }
}