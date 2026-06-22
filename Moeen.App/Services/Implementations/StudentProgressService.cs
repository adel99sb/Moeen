using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses.Mobile;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class StudentProgressService : IStudentProgressService
    {
        private readonly StudentProgressApiClient _apiClient;
        private readonly AppSessionService _session;

        public StudentProgressService(StudentProgressApiClient apiClient, AppSessionService session)
        {
            _apiClient = apiClient;
            _session = session;
        }

        public async Task<StudentProgressResponse> GetMyProgressAsync(DateTime? from = null, DateTime? to = null, ProgressRecordType? type = null)
        {
            var response = await _apiClient.GetMyProgressAsync(_session.AuthToken ?? string.Empty, from, to, type);
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل جلب سجل التقدم.");

            var json = JsonSerializer.Serialize(response.Data);
            var data = JsonSerializer.Deserialize<StudentProgressResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data ?? throw new Exception("استجابة سجل التقدم غير صالحة.");
        }
    }
}
