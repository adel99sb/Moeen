using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class EnrollmentApiClient
    {
        private readonly HttpClient _httpClient;
        public EnrollmentApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetMemberProfileAsync(string memberId)
        {
            var url = string.Format(ApiRoutes.GetMemberProfileRoute, memberId);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetChildrenByParentAsync(Guid parentId)
        {
            var url = string.Format(ApiRoutes.GetChildrenByParentRoute, parentId);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
