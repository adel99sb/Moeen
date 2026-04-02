using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Feedback;
using Moeen.Api.Shared.Responses.Feedback;

namespace Moeen.Api.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        public Task<ManageFeedbackResponse> ManageFeedbacksAsync(ManageFeedbackRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SubmitComplaintResponse> SubmitComplaintAsync(SubmitComplaintRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SubmitSuggestionResponse> SubmitSuggestionAsync(SubmitSuggestionRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
