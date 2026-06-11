using System.Net.Http.Json;
using System.Text.Json;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Mosque;
using Moeen.Shared.Requests.User;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mosque;

namespace Moeen.Dashboard.Services.Implementations
{
    public sealed class MosqueApiService : IMosqueApiService
    {
        private readonly HttpClient _httpClient;

        public MosqueApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<MosqueResponse>> GetAllMosquesAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<GeneralResponse>("api/Mosque", cancellationToken);
            if (response?.Data is not JsonElement element || element.ValueKind != JsonValueKind.Array)
            {
                return Array.Empty<MosqueResponse>();
            }

            return element.Deserialize<List<MosqueResponse>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? [];
        }

        public async Task<MosqueResponse?> GetMosqueByIdAsync(Guid mosqueId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<GeneralResponse>($"api/Mosque/{mosqueId}", cancellationToken);
            if (response?.Data is not JsonElement element || element.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            return element.Deserialize<MosqueResponse>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<IReadOnlyList<UserResponse>> GetMosqueUsersAsync(Guid mosqueId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<GeneralResponse>($"api/Mosque/{mosqueId}/users", cancellationToken);
            if (response?.Data is not JsonElement element || element.ValueKind != JsonValueKind.Array)
            {
                return Array.Empty<UserResponse>();
            }

            return element.Deserialize<List<UserResponse>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? [];
        }

        public async Task<string?> UpdateMosqueAsync(Guid mosqueId, CreateMosqueRequest request, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PutAsJsonAsync($"api/Mosque/{mosqueId}", request, cancellationToken);
            var body = await response.Content.ReadFromJsonAsync<GeneralResponse>(cancellationToken: cancellationToken);
            return body?.Message ?? (response.IsSuccessStatusCode ? "Mosque updated successfully." : "Failed to update mosque.");
        }

        public async Task<string?> CreateMosqueAsync(CreateMosqueRequest request, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsJsonAsync("api/Mosque", request, cancellationToken);
            var body = await response.Content.ReadFromJsonAsync<GeneralResponse>(cancellationToken: cancellationToken);
            return body?.Message ?? (response.IsSuccessStatusCode ? "Mosque created successfully." : "Failed to create mosque.");
        }

        public async Task<string?> DeleteMosqueAsync(Guid mosqueId, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.DeleteAsync($"api/Mosque/{mosqueId}", cancellationToken);
            var body = await response.Content.ReadFromJsonAsync<GeneralResponse>(cancellationToken: cancellationToken);
            return body?.Message ?? (response.IsSuccessStatusCode ? "Mosque deleted successfully." : "Failed to delete mosque.");
        }
    }
}
