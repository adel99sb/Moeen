using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Mobile;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class ParentProfileService : IParentProfileService
    {
        private readonly ParentProfileApiClient _apiClient;
        private readonly AppSessionService _session;

        public ParentProfileService(ParentProfileApiClient apiClient, AppSessionService session)
        {
            _apiClient = apiClient;
            _session = session;
        }

        public async Task<ParentProfileResponse> GetMyProfileAsync()
        {
            var response = await _apiClient.GetMyProfileAsync(_session.AuthToken ?? string.Empty);
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل جلب ملف ولي الأمر.");

            var json = JsonSerializer.Serialize(response.Data);
            var data = JsonSerializer.Deserialize<ParentProfileResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data ?? new ParentProfileResponse();
        }

        public async Task SubmitNoteAsync(string content)
        {
            var response = await _apiClient.SubmitNoteAsync(_session.AuthToken ?? string.Empty, content);
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل إرسال الملاحظة.");
        }
    }
}
