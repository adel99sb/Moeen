using Moeen.Shared.Requests;
using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IFeedbackService
    {
        Task<GeneralResponse> SubmitComplaintAsync(SubmitComplaintRequest request);
        Task<GeneralResponse> SubmitSuggestionAsync(SubmitSuggestionRequest request);
        Task<GeneralResponse> ManageFeedbacksAsync(ManageFeedbackRequest request);
        Task<GeneralResponse> UpdateComplaintStatusAsync(UpdateComplaintStatusRequest request);
        Task<GeneralResponse> GetComplaintsAsync(PaginationRequest request);
        Task<GeneralResponse> GetSuggestionsAsync(PaginationRequest request);
        Task<GeneralResponse> UpdateSuggestionStatusAsync(UpdateSuggestionStatusRequest request);
        Task<GeneralResponse> TransferToOwnerAsync(Guid feedbackId);
        Task<GeneralResponse> DeleteComplaintAsync(Guid complaintId);
    }
}
