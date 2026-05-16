using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class AuthApiClient
    {
        private readonly HttpClient _http;

        public AuthApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<GeneralResponse> Login(LoginRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.LoginRoute, request);

            var content = await response.Content.ReadFromJsonAsync<GeneralResponse>();

            return content;
        }
        public async Task<GeneralResponse> CreateUser(RegisterRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.registerRoute, request);

            var content = await response.Content.ReadFromJsonAsync<GeneralResponse>();

            return content;
        }

    }
}
