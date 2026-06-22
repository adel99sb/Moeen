using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Mobile;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class ParentPostService : IParentPostService
    {
        private readonly ParentPostApiClient _apiClient;
        private readonly AppSessionService _session;

        public ParentPostService(ParentPostApiClient apiClient, AppSessionService session)
        {
            _apiClient = apiClient;
            _session = session;
        }

        public async Task<List<ParentPostDto>> GetMyPostsAsync()
        {
            var response = await _apiClient.GetMyPostsAsync(_session.AuthToken ?? string.Empty);
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل جلب منشورات ولي الأمر.");

            var json = JsonSerializer.Serialize(response.Data);
            var data = JsonSerializer.Deserialize<ParentPostResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data?.Posts ?? new List<ParentPostDto>();
        }

        public async Task<ParentPostInteractionResponse> ToggleLikeAsync(Guid postId)
        {
            var response = await _apiClient.ToggleLikeAsync(_session.AuthToken ?? string.Empty, postId);
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل تحديث التفاعل مع المنشور.");

            var json = JsonSerializer.Serialize(response.Data);
            var data = JsonSerializer.Deserialize<ParentPostInteractionResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data ?? throw new Exception("استجابة التفاعل مع المنشور غير صالحة.");
        }
    }
}
