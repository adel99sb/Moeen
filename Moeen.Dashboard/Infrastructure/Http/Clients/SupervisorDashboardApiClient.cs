using Moeen.Shared.Responses.SupervisorDashboard;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class SupervisorDashboardApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SupervisorDashboardApiClient> _logger;

        public SupervisorDashboardApiClient(HttpClient httpClient, ILogger<SupervisorDashboardApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<SupervisorDashboardResponse?> GetOverviewAsync(string token)
        {
            var route = $"{ApiRoutes.SupervisorDashboardOverviewRoute}?_={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            using var request = new HttpRequestMessage(HttpMethod.Get, route);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true
            };

            _logger.LogInformation(
                "Supervisor dashboard overview request prepared with explicit bearer token. HasToken={HasToken}, TokenLength={TokenLength}",
                !string.IsNullOrWhiteSpace(token),
                token?.Length ?? 0);

            using var response = await _httpClient.SendAsync(request);

            _logger.LogInformation(
                "Supervisor dashboard overview response received. StatusCode={StatusCode}",
                (int)response.StatusCode);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SupervisorDashboardResponse>();
        }
    }
}
