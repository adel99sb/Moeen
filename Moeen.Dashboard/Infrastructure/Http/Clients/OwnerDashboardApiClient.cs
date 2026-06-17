using Moeen.Shared.Responses.OwnerDashboard;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class OwnerDashboardApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OwnerDashboardApiClient> _logger;

        public OwnerDashboardApiClient(HttpClient httpClient, ILogger<OwnerDashboardApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<OwnerDashboardResponse?> GetOverviewAsync(string token)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, ApiRoutes.OwnerDashboardOverviewRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            _logger.LogInformation("Owner dashboard overview request prepared. HasToken={HasToken}", !string.IsNullOrWhiteSpace(token));

            using var response = await _httpClient.SendAsync(request);
            _logger.LogInformation("Owner dashboard overview response received. StatusCode={StatusCode}", (int)response.StatusCode);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OwnerDashboardResponse>();
        }
    }
}
