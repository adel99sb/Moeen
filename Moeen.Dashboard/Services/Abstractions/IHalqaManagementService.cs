using Moeen.Shared.Requests.Halqa;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IHalqaManagementService
    {
        Task<GeneralResponse> GetAllAsync(Guid? mosqueId = null);
        Task<GeneralResponse> GetAssignmentStudentsAsync(Guid? halqaId = null);
        Task<GeneralResponse> CreateAsync(CreateHalqaRequest request);
        Task<GeneralResponse> UpdateAsync(UpdateHalqaRequest request);
        Task<GeneralResponse> DeleteAsync(DeleteHalqaRequest request);
    }
}
