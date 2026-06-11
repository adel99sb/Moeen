using Moeen.Shared.Constants;
using Moeen.Shared.Responses.Complaint;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IComplaintApiService
    {
        Task<IReadOnlyList<ComplaintResponse>> GetAllComplaintsAsync(CancellationToken cancellationToken = default);
        Task<string?> UpdateComplaintStatusAsync(Guid complaintId, ComplaintStatus status, CancellationToken cancellationToken = default);
        Task<string?> DeleteComplaintAsync(Guid complaintId, CancellationToken cancellationToken = default);
    }
}
