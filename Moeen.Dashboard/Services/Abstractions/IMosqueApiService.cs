using Moeen.Shared.Responses.Mosque;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IMosqueApiService
    {
        Task<IReadOnlyList<MosqueResponse>> GetAllMosquesAsync(CancellationToken cancellationToken = default);
        Task<string?> DeleteMosqueAsync(Guid mosqueId, CancellationToken cancellationToken = default);
    }
}
