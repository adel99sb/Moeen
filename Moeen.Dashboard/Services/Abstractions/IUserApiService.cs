using Moeen.Shared.Constants;
using Moeen.Shared.Requests.User;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IUserApiService
    {
        Task<IReadOnlyList<UserResponse>> GetUsersByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
        Task<string?> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
