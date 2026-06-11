using System.Net.Http.Json;
using System.Text.Json;
using Moeen.Dashboard.Services.Abstractions;
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

            var mosques = element.Deserialize<List<MosqueResponse>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? [];

            return mosques;
        }

        public async Task<string?> DeleteMosqueAsync(Guid mosqueId, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.DeleteAsync($"api/Mosque/{mosqueId}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }

            var body = await response.Content.ReadFromJsonAsync<GeneralResponse>(cancellationToken: cancellationToken);
            return body?.Message;
        }
    }
}
