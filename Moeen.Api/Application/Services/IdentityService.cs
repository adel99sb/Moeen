using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Entities;
using Moeen.Api.Shared.Requests.Identity;
using Moeen.Api.Shared.Responses.Identity;

namespace Moeen.Api.Application.Services
{
    public class IdentityService : IIdentityService
    {
        public Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetCurrentUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> RegisterAsync(RegisterRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> UpdateProfileAsync(UpdateProfileRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
