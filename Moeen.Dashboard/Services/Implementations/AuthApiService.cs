using System.Net.Http.Json;
using System.Text.Json;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.User;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.User;

namespace Moeen.Dashboard.Services.Implementations
{
    public sealed class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _httpClient;

        public AuthApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool Success, string Message, LoginResponse? Data)> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsJsonAsync("api/User/login", request, cancellationToken);
            var body = await ReadGeneralResponseAsync(response, cancellationToken);
            var data = DeserializeData<LoginResponse>(body?.Data);
            return (body?.Success ?? false, body?.Message ?? "Login failed.", data);
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            using var content = new MultipartFormDataContent
            {
                { new StringContent(request.fullName ?? string.Empty), nameof(request.fullName) },
                { new StringContent(request.email ?? string.Empty), nameof(request.email) },
                { new StringContent(request.Phone ?? string.Empty), nameof(request.Phone) },
                { new StringContent(request.password ?? string.Empty), nameof(request.password) },
                { new StringContent(((int)request.userType).ToString()), nameof(request.userType) }
            };

            if (request.Picture is not null)
            {
                using var stream = request.Picture.OpenReadStream();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms, cancellationToken);
                var fileContent = new ByteArrayContent(ms.ToArray());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.Picture.ContentType);
                content.Add(fileContent, nameof(request.Picture), request.Picture.FileName);
            }

            using var response = await _httpClient.PostAsync("api/User/register", content, cancellationToken);
            var body = await ReadGeneralResponseAsync(response, cancellationToken);
            return (body?.Success ?? false, body?.Message ?? "Registration failed.");
        }

        public async Task<(bool Success, string Message)> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsJsonAsync("api/User/verify-email", request, cancellationToken);
            var body = await ReadGeneralResponseAsync(response, cancellationToken);
            return (body?.Success ?? false, body?.Message ?? "Verification failed.");
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsJsonAsync("api/User/reset-password", request, cancellationToken);
            var body = await ReadGeneralResponseAsync(response, cancellationToken);
            return (body?.Success ?? false, body?.Message ?? "Reset password failed.");
        }

        public async Task<(bool Success, string Message)> SendVerifyCodeAsync(SendVerifyEmailCodeRequest request, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsJsonAsync("api/User/send-verify-code", request, cancellationToken);
            var body = await ReadGeneralResponseAsync(response, cancellationToken);
            return (body?.Success ?? false, body?.Message ?? "Send verification code failed.");
        }

        private static async Task<GeneralResponse?> ReadGeneralResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            return await response.Content.ReadFromJsonAsync<GeneralResponse>(cancellationToken: cancellationToken);
        }

        private static T? DeserializeData<T>(object data)
        {
            if (data is null) return default;
            if (data is JsonElement element)
            {
                return element.Deserialize<T>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            var json = JsonSerializer.Serialize(data);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
