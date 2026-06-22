using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses.Mobile;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class ParentProgressService : IParentProgressService
    {
        private readonly ParentProgressApiClient _apiClient;
        private readonly AppSessionService _session;

        public ParentProgressService(ParentProgressApiClient apiClient, AppSessionService session)
        {
            _apiClient = apiClient;
            _session = session;
        }

        public async Task<ParentProgressResponse> GetMyChildProgressAsync(Guid? childId = null, DateTime? from = null, DateTime? to = null, ProgressRecordType? type = null)
        {
            var response = await _apiClient.GetMyChildProgressAsync(_session.AuthToken ?? string.Empty, childId, from, to, type);
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل جلب تقدم الطالب لولي الأمر.");

            var json = JsonSerializer.Serialize(response.Data);
            var data = JsonSerializer.Deserialize<ParentProgressResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data ?? new ParentProgressResponse();
        }
    }
}
