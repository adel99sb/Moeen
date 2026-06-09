using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class ContentShaeringApiClient
    {
        private readonly HttpClient _httpClient;
        public ContentShaeringApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<GeneralResponse> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(ApiRoutes.GetAllContentSharingRoute);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
