using Moeen.Shared.Requests.Fouj;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IFoujManagementService
    {
        Task<GeneralResponse> GetAllAsync(GetAllFoujsRequest request);
        Task<GeneralResponse> CreateAsync(CreateFoujRequest request);
        Task<GeneralResponse> UpdateAsync(UpdateFoujRequest request);
        Task<GeneralResponse> DeleteAsync(Guid foujId);
    }
}
