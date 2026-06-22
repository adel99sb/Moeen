using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Mobile;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class ParentDashboardService : IParentDashboardService
    {
        private readonly ParentDashboardApiClient _apiClient;
        private readonly AppSessionService _session;

        public ParentDashboardService(ParentDashboardApiClient apiClient, AppSessionService session)
        {
            _apiClient = apiClient;
            _session = session;
        }

        public async Task<ParentDashboardResponse> GetMyDashboardAsync(Guid? childId = null)
        {
            var response = await _apiClient.GetMyDashboardAsync(_session.AuthToken ?? string.Empty, childId);
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل جلب لوحة ولي الأمر.");

            var json = JsonSerializer.Serialize(response.Data);
            var data = JsonSerializer.Deserialize<ParentDashboardResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data ?? new ParentDashboardResponse();
        }
    }
}
