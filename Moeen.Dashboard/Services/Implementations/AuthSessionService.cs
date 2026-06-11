using System.Text.Json;
using Microsoft.JSInterop;
using Moeen.Dashboard.Services.Abstractions;

namespace Moeen.Dashboard.Services.Implementations
{
    public sealed class AuthSessionService : IAuthSessionService
    {
        private readonly IJSRuntime _js;

        public AuthSessionService(IJSRuntime js)
        {
            _js = js;
        }

        public string? AccessToken { get; private set; }
        public string? Role { get; private set; }
        public bool IsEmailConfirmed { get; private set; }
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken);

        public async Task InitializeAsync()
        {
            var token = await _js.InvokeAsync<string?>("eval", "localStorage.getItem('authToken')");
            if (!string.IsNullOrWhiteSpace(token))
            {
                SetLocalToken(token);
            }
        }

        public async Task SetTokenAsync(string token)
        {
            await _js.InvokeVoidAsync("eval", $"localStorage.setItem('authToken', {JsonSerializer.Serialize(token)})");
            SetLocalToken(token);
        }

        public async Task ClearAsync()
        {
            await _js.InvokeVoidAsync("eval", "localStorage.removeItem('authToken')");
            AccessToken = null;
            Role = null;
            IsEmailConfirmed = false;
        }

        private void SetLocalToken(string token)
        {
            AccessToken = token;
            var claims = ReadClaims(token);
            Role = claims.Role;
            IsEmailConfirmed = claims.EmailConfirmed;
        }

        private static AuthClaims ReadClaims(string token)
        {
            var parts = token.Split('.');
            if (parts.Length < 2)
            {
                return new AuthClaims();
            }

            var payload = parts[1].PadRight(parts[1].Length + (4 - parts[1].Length % 4) % 4, '=')
                                  .Replace('-', '+')
                                  .Replace('_', '/');
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return new AuthClaims
            {
                Role = GetString(root, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"),
                EmailConfirmed = GetBool(root, "EmailConfirmed")
            };
        }

        private static string? GetString(JsonElement root, string key)
            => root.TryGetProperty(key, out var prop) ? prop.GetString() : null;

        private static bool GetBool(JsonElement root, string key)
            => root.TryGetProperty(key, out var prop) && bool.TryParse(prop.GetString(), out var value) && value;

        private sealed class AuthClaims
        {
            public string? Role { get; set; }
            public bool EmailConfirmed { get; set; }
        }
    }
}
