using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Requests.ContentSharing;
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
            try
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
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task InteractWithPostAsync(InteractWithPostRequest request)
        {
            try
            {
                var res = await _apiClient.InteractWithPostAsync(request);
                if (res == null || !res.Success)
                    throw new Exception(res?.Message ?? "Unknown error");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
