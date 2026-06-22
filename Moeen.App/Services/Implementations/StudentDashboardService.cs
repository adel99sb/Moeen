using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Mobile;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class StudentDashboardService : IStudentDashboardService
    {
        private readonly StudentDashboardApiClient _apiClient;
        private readonly AppSessionService _session;

        public StudentDashboardService(StudentDashboardApiClient apiClient, AppSessionService session)
        {
            _apiClient = apiClient;
            _session = session;
        }

        public async Task<StudentDashboardResponse> GetMyDashboardAsync()
        {
            var response = await _apiClient.GetMyDashboardAsync(_session.AuthToken ?? string.Empty);
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل جلب بيانات الصفحة الرئيسية.");

            var json = JsonSerializer.Serialize(response.Data);
            var data = JsonSerializer.Deserialize<StudentDashboardResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data ?? throw new Exception("استجابة الصفحة الرئيسية غير صالحة.");
        }
    }
}
