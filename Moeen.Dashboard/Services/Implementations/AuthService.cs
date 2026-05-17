using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Responses; // تم إضافة هذا السطر لحل مشكلة الـ GeneralResponse
using Moeen.Shared.Responses.Identity;
using System.Text.Json;

namespace Moeen.Dashboard.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AuthApiClient _client;

        public AuthService(AuthApiClient client)
        {
            _client = client;
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

            return data;
        }

        public async Task Register(RegisterRequest request)
        {
            var res = await _client.Register(request);

            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Registration failed");
        }

        public async Task SendResetUrl(SendPasswordResetUrlRequest request)
        {
            var res = await _client.SendResetUrl(request);

            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Failed to send reset url");
        }

        public async Task ResetPassword(ChangePasswordRequest request)
        {
            var res = await _client.ResetPassword(request);

            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Reset password failed");
        }

        public async Task SendVerifyCode(SendVerifyEmailCodeRequest request)
        {
            var res = await _client.SendVerifyCode(request);

            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Failed to send verify code");
        }

        public async Task VerifyEmail(VerifyEmailRequest request)
        {
            var res = await _client.VerifyEmail(request);

            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Verify email failed");
        }
    }
}