using Moeen.Shared.Requests.Identity;

namespace Moeen.App.Services.Abstractions
{
    public interface IAppAuthService
    {
        Task<string> LoginAsync(LoginRequest request, AppUserMode expectedMode);
    }
}
