using Moeen.Shared.Requests.Fouj;
using Moeen.Shared.Responses;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IFoujService
    {
        Task<GeneralResponse> CreateFoujAsync(CreateFoujRequest request);
        Task<GeneralResponse> UpdateFoujAsync(UpdateFoujRequest request);
        Task<GeneralResponse> DeleteFoujAsync(DeleteFoujRequest request);
        Task<GeneralResponse> GetAllFoujsAsync(GetAllFoujsRequest request);
        Task<GeneralResponse> AddHalqaToFoujAsync(AddHalqaToFoujRequest request);
        Task<GeneralResponse> RemoveHalqaFromFoujAsync(RemoveHalqaFromFoujRequest request);
    }
}