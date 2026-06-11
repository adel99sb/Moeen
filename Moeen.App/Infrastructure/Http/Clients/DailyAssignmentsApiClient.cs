using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class DailyAssignmentsApiClient
    {
        private readonly HttpClient _httpClient;
        public DailyAssignmentsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetByStudentAsync(Guid studentId, DateTime? date = null, Guid? halqaId = null)
        {
            var url = string.Format(ApiRoutes.GetStudentDailyAssignmentsRoute, studentId);

            var query = new List<string>();
            if (date.HasValue)
                query.Add($"date={Uri.EscapeDataString(date.Value.ToString("o"))}");
            if (halqaId.HasValue)
                query.Add($"halqaId={halqaId.Value}");

            if (query.Any())
                url += "?" + string.Join("&", query);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
