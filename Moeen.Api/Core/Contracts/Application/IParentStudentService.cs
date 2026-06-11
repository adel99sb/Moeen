using Moeen.Shared.Requests.ParentStudent;
using Moeen.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IParentStudentService
    {
        Task<GeneralResponse> LinkParentToStudentAsync(LinkParentStudentRequest request);
        Task<GeneralResponse> UnlinkParentFromStudentAsync(Guid parentId, Guid studentId);
        Task<GeneralResponse> GetStudentsByParentIdAsync(Guid parentId);
        Task<GeneralResponse> GetParentsByStudentIdAsync(Guid studentId);
    }
}
