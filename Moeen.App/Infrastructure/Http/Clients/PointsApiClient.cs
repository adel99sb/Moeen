using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class PointsApiClient
    {
        private readonly HttpClient _httpClient;
        public PointsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetStudentPointsAsync(Guid studentId)
        {
            var url = string.Format(ApiRoutes.GetStudentPointsRoute, studentId);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetStudentPointsBreakdownAsync(Guid studentId)
        {
            var url = string.Format(ApiRoutes.GetStudentPointsBreakdownRoute, studentId);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
