using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Mobile;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class StudentPostService : IStudentPostService
    {
        private readonly StudentPostApiClient _apiClient;
        private readonly AppSessionService _session;

        public StudentPostService(StudentPostApiClient apiClient, AppSessionService session)
        {
            _apiClient = apiClient;
            _session = session;
        }

        public async Task<List<StudentPostDto>> GetMyPostsAsync()
        {
            var response = await _apiClient.GetMyPostsAsync(_session.AuthToken ?? string.Empty);
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل جلب المنشورات.");

            var json = JsonSerializer.Serialize(response.Data);
            var data = JsonSerializer.Deserialize<StudentPostResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data?.Posts ?? new List<StudentPostDto>();
        }

        public async Task<StudentPostInteractionResponse> ToggleLikeAsync(Guid postId)
        {
            var response = await _apiClient.ToggleLikeAsync(_session.AuthToken ?? string.Empty, postId);
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل تحديث التفاعل.");

            var json = JsonSerializer.Serialize(response.Data);
            var data = JsonSerializer.Deserialize<StudentPostInteractionResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data ?? throw new Exception("استجابة التفاعل غير صالحة.");
        }
    }
}
