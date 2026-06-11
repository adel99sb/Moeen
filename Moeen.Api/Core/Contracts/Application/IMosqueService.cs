using Moeen.Shared.Requests.Mosque;
using Moeen.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IMosqueService
    {
        // عمليات التحكم بالجامع (CRUD)
        Task<GeneralResponse> CreateMosqueAsync(CreateMosqueRequest createMosqueRequest);
        Task<GeneralResponse> UpdateMosqueAsync(Guid mosqueId, CreateMosqueRequest updateMosqueRequest);
        Task<GeneralResponse> DeleteMosqueAsync(Guid mosqueId);
        Task<GeneralResponse> GetMosqueByIdAsync(Guid mosqueId);
        Task<GeneralResponse> GetAllMosquesAsync();

        // عمليات إدارة مستخدمي الجامع (MosqueUsers)
        Task<GeneralResponse> AddUserToMosqueAsync(AddUserToMosqueRequest addUserToMosqueRequest);
        Task<GeneralResponse> RemoveUserFromMosqueAsync(Guid mosqueId, Guid userId);
        Task<GeneralResponse> GetMosqueUsersAsync(Guid mosqueId);
    }
}
