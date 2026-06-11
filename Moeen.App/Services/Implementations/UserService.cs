using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Identity;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserApiClient _apiClient;

        public UserService(UserApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var res = await _apiClient.GetByIdAsync(userId);
            if (res == null || !res.Success)
                return null;

            var json = JsonSerializer.Serialize(res.Data);
            var data = JsonSerializer.Deserialize<UserDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return data;
        }
    }
}
