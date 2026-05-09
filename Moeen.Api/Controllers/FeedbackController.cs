using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses;
using Moeen.Shared.Requests;

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
        public async Task<ActionResult<GeneralResponse>> SubmitComplaint([FromBody] SubmitComplaintRequest request)
            => Ok(await _feedbackService.SubmitComplaintAsync(request));

        /// <summary>
        /// أمر: تقديم اقتراح.
        /// </summary>
        [HttpPost("suggestion")]
        public async Task<ActionResult<GeneralResponse>> SubmitSuggestion([FromBody] SubmitSuggestionRequest request)
            => Ok(await _feedbackService.SubmitSuggestionAsync(request));

        /// <summary>
        /// أمر: إدارة الشكاوى والاقتراحات.
        /// </summary>
        [HttpPut("manage")]
        public async Task<ActionResult<GeneralResponse>> ManageFeedback([FromBody] ManageFeedbackRequest request)
            => Ok(await _feedbackService.ManageFeedbacksAsync(request));

        /// <summary>
        /// PUT: تحديث حالة الشكوى.
        /// </summary>
        [HttpPut("complaint/status")]
        public async Task<ActionResult<GeneralResponse>> UpdateComplaintStatus([FromBody] UpdateComplaintStatusRequest request)
            => Ok(await _feedbackService.UpdateComplaintStatusAsync(request));

        /// <summary>
        /// GET: استرجاع قائمة الشكاوى مع ترقيم.
        /// </summary>
        [HttpGet("complaints")]
        public async Task<ActionResult<GeneralResponse>> GetComplaints([FromQuery] PaginationRequest request)
            => Ok(await _feedbackService.GetComplaintsAsync(request ?? new PaginationRequest()));

        /// <summary>
        /// GET: استرجاع قائمة الاقتراحات مع ترقيم.
        /// </summary>
        [HttpGet("suggestions")]
        public async Task<ActionResult<GeneralResponse>> GetSuggestions([FromQuery] PaginationRequest request)
            => Ok(await _feedbackService.GetSuggestionsAsync(request ?? new PaginationRequest()));

        /// <summary>
        /// PUT: تحديث حالة الاقتراح.
        /// </summary>
        [HttpPut("suggestion/status")]
        public async Task<ActionResult<GeneralResponse>> UpdateSuggestionStatus([FromBody] UpdateSuggestionStatusRequest request)
            => Ok(await _feedbackService.UpdateSuggestionStatusAsync(request));
    }
}