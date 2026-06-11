using System.Net.Http.Json;
using System.Text.Json;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.User;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementations
{
    public sealed class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;

        public UserApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<UserResponse>> GetUsersByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<GeneralResponse>("api/User", cancellationToken);
            if (response?.Data is not JsonElement element || element.ValueKind != JsonValueKind.Array)
            {
                return Array.Empty<UserResponse>();
            }

            var users = element.Deserialize<List<UserResponse>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? [];

            return users.Where(user => user.UserRole == role).ToList();
        }

        public async Task<string?> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.DeleteAsync($"api/User/{userId}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }

            var body = await response.Content.ReadFromJsonAsync<GeneralResponse>(cancellationToken: cancellationToken);
            return body?.Message;
        }
    }
}
