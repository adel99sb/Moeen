using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class FeedbackApiClient
    {
        private readonly HttpClient _httpClient;
        public FeedbackApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> SubmitComplaintAsync(SubmitComplaintRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.SubmitComplaintRoute, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
