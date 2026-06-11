using Microsoft.AspNetCore.Identity;
using Moeen.Api.Core.Entities;

namespace Moeen.Api.Core.Contracts.infrastructure.Providers
{
    public interface IJwtService
    {
        Task<string> GenerateJwtToken(User user, UserManager<User> userManager);
    }
}