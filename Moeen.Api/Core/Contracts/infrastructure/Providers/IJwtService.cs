using Moeen.Api.Core.Entities;

namespace Moeen.Api.Core.Contracts.infrastructure.Providers
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(User user);
    }
}