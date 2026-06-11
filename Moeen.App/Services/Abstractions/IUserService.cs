using Moeen.Shared.Responses.Identity;

namespace Moeen.App.Services.Abstractions
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(Guid userId);
    }
}
