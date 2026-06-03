using System.Threading.Tasks;
using Moeen.Shared.Requests.DailyAssignments;
using Moeen.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IDailyAssignmentService
    {
        Task<GeneralResponse> GetDailyAssignmentsAsync(GetDailyAssignmentsRequest request);
        Task<GeneralResponse> UpdateAssignmentStatusAsync(UpdateAssignmentStatusRequest request);
        Task<GeneralResponse> AddDailyAssignmentAsync(AddDailyAssignmentRequest request);
    }
}