using Moeen.Shared.Requests.Mosque;
using Moeen.Shared.Requests.User;
using Moeen.Shared.Responses.Mosque;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IMosqueApiService
    {
        Task<IReadOnlyList<MosqueResponse>> GetAllMosquesAsync(CancellationToken cancellationToken = default);
        Task<MosqueResponse?> GetMosqueByIdAsync(Guid mosqueId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<UserResponse>> GetMosqueUsersAsync(Guid mosqueId, CancellationToken cancellationToken = default);
        Task<string?> UpdateMosqueAsync(Guid mosqueId, CreateMosqueRequest request, CancellationToken cancellationToken = default);
        Task<string?> CreateMosqueAsync(CreateMosqueRequest request, CancellationToken cancellationToken = default);
        Task<string?> DeleteMosqueAsync(Guid mosqueId, CancellationToken cancellationToken = default);
    }
}
