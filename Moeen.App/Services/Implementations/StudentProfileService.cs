using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Mobile;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class StudentProfileService : IStudentProfileService
    {
        private readonly StudentProfileApiClient _apiClient;
        private readonly AppSessionService _session;

        public StudentProfileService(StudentProfileApiClient apiClient, AppSessionService session)
        {
            _apiClient = apiClient;
            _session = session;
        }

        public async Task<StudentProfileResponse> GetMyProfileAsync()
        {
            var response = await _apiClient.GetMyProfileAsync(_session.AuthToken ?? string.Empty);
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل جلب الملف الشخصي.");

            var json = JsonSerializer.Serialize(response.Data);
            var data = JsonSerializer.Deserialize<StudentProfileResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data ?? throw new Exception("استجابة الملف الشخصي غير صالحة.");
        }

        public async Task SubmitNoteAsync(string content)
        {
            var response = await _apiClient.SubmitNoteAsync(_session.AuthToken ?? string.Empty, content);
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل إرسال الملاحظة.");
        }
    }
}
