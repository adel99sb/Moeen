using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Responses.Identity;
using System.Text.Json;

namespace Moeen.Dashboard.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AuthApiClient _client;
        //private readonly ITokenService _token;

        public AuthService(AuthApiClient client/*, ITokenService token*/)
        {
            _client = client;
          //  _token = token;
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            var res = await _client.Login(request);

            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Unknown error");

            var json = JsonSerializer.Serialize(res.Data);

            var data = JsonSerializer.Deserialize<AuthResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            //await _token.Save(data.AccessToken);

            return data;
        }
        public async Task Register(RegisterRequest request)
        {
            var res = await _client.CreateUser(request);

            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Unknown error");
            //await _token.Save(data.AccessToken);
        }
    }
}
