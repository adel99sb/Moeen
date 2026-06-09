using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.ContentSharing;
using Moeen.Shared.Responses.Mosuq;
using System.Collections.Generic;
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
            return data;
        }
    }
}
