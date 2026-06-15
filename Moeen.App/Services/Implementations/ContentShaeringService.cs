using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses.ContentSharing;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class ContentShaeringService : IContentShaeringService
    {
        private readonly ContentShaeringApiClient _apiClient;

        public ContentShaeringService(
            ContentShaeringApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<PostDto>> GetAllAsync()
        {
            var res = await _apiClient.GetAllAsync();
            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Unknown error");

            var json = JsonSerializer.Serialize(res.Data);

            var data = JsonSerializer.Deserialize<List<PostDto>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            return data ?? new List<PostDto>();
        }

        public async Task InteractAsync(Guid postId, InteractionType type)
        {
            var res = await _apiClient.InteractAsync(new InteractWithPostRequest
            {
                PostId = postId,
                Type = type
            });

            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "فشل تسجيل التفاعل");
        }
    }
}
