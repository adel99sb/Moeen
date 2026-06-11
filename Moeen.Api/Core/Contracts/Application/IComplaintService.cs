using Moeen.Shared.Requests.Complaint;
using Moeen.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IComplaintService
    {
        Task<GeneralResponse> CreateComplaintAsync(CreateComplaintRequest createComplaintRequest);
        Task<GeneralResponse> UpdateComplaintStatusAsync(Guid complaintId, UpdateComplaintStatusRequest updateStatusRequest);
        Task<GeneralResponse> DeleteComplaintAsync(Guid complaintId);
        Task<GeneralResponse> GetComplaintByIdAsync(Guid complaintId);
        Task<GeneralResponse> GetAllComplaintsAsync();

        Task<GeneralResponse> GetComplaintsByUserIdAsync(Guid userId);
    }
}