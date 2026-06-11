using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class AttendanceApiClient
    {
        private readonly HttpClient _httpClient;
        public AttendanceApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetStudentAttendanceRateAsync(Guid studentId)
        {
            var url = string.Format(ApiRoutes.GetStudentAttendanceRateRoute, studentId);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetStudentAbsenceReportAsync(Guid studentId)
        {
            var url = string.Format(ApiRoutes.GetStudentAbsenceReportRoute, studentId);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
