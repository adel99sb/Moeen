using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost("complaint")]
        public async Task<ActionResult<GeneralResponse>> SubmitComplaint([FromBody] SubmitComplaintRequest request)
            => Ok(await _feedbackService.SubmitComplaintAsync(request));

        [HttpPost("suggestion")]
        public async Task<ActionResult<GeneralResponse>> SubmitSuggestion([FromBody] SubmitSuggestionRequest request)
            => Ok(await _feedbackService.SubmitSuggestionAsync(request));

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPut("manage")]
        public async Task<ActionResult<GeneralResponse>> ManageFeedback([FromBody] ManageFeedbackRequest request)
            => Ok(await _feedbackService.ManageFeedbacksAsync(request));

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPut("complaint/status")]
        public async Task<ActionResult<GeneralResponse>> UpdateComplaintStatus([FromBody] UpdateComplaintStatusRequest request)
            => Ok(await _feedbackService.UpdateComplaintStatusAsync(request));

        [Authorize(Roles = "Admin,Owner,Supervisor")]
        [HttpGet("complaints")]
        public async Task<ActionResult<GeneralResponse>> GetComplaints([FromQuery] PaginationRequest request)
            => Ok(await _feedbackService.GetComplaintsAsync(request ?? new PaginationRequest()));

        [Authorize(Roles = "Admin,Owner,Supervisor")]
        [HttpGet("suggestions")]
        public async Task<ActionResult<GeneralResponse>> GetSuggestions([FromQuery] PaginationRequest request)
            => Ok(await _feedbackService.GetSuggestionsAsync(request ?? new PaginationRequest()));

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPut("suggestion/status")]
        public async Task<ActionResult<GeneralResponse>> UpdateSuggestionStatus([FromBody] UpdateSuggestionStatusRequest request)
            => Ok(await _feedbackService.UpdateSuggestionStatusAsync(request));

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPut("{feedbackId:guid}/transfer-to-owner")]
        public async Task<ActionResult<GeneralResponse>> TransferToOwner([FromRoute] Guid feedbackId)
            => Ok(await _feedbackService.TransferToOwnerAsync(feedbackId));

        [Authorize(Roles = "Admin,Owner")]
        [HttpDelete("{complaintId:guid}")]
        public async Task<ActionResult<GeneralResponse>> DeleteComplaint([FromRoute] Guid complaintId)
            => Ok(await _feedbackService.DeleteComplaintAsync(complaintId));
    }
}
