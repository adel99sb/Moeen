using System.Collections.Generic;
using System.Threading.Tasks;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses;

namespace Moeen.Frontend.Services.Abstractions
{
    public interface IFeedbackService
    {
        Task<GeneralResponse> SubmitComplaintAsync(SubmitComplaintRequest request);
        Task<GeneralResponse> SubmitSuggestionAsync(SubmitSuggestionRequest request);
        Task<GeneralResponse> ManageFeedbackAsync(ManageFeedbackRequest request);
        Task<GeneralResponse> UpdateComplaintStatusAsync(UpdateComplaintStatusRequest request);
        Task<GeneralResponse> UpdateSuggestionStatusAsync(UpdateSuggestionStatusRequest request);
        Task<List<ComplaintDto>> GetComplaintsAsync(PaginationRequest request);
        Task<GeneralResponse> GetComplaintsResponseAsync(PaginationRequest request);
        Task<GeneralResponse> GetSuggestionsAsync(PaginationRequest request);
        Task<GeneralResponse> TransferToOwnerAsync(Guid feedbackId);
        Task<GeneralResponse> DeleteFeedbackAsync(Guid complaintId);
    }
}
