using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Responses.Identity;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class AppAuthService : IAppAuthService
    {
        private readonly AuthApiClient _client;

        public AppAuthService(AuthApiClient client)
        {
            _client = client;
        }

        public async Task<string> LoginAsync(LoginRequest request, AppUserMode expectedMode)
        {
            var response = await _client.LoginAsync(request);

            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل تسجيل الدخول.");

            var json = JsonSerializer.Serialize(response.Data);
            var auth = JsonSerializer.Deserialize<AuthResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (auth == null || string.IsNullOrWhiteSpace(auth.Token))
                throw new Exception("تم تسجيل الدخول لكن السيرفر لم يرجع token صالح.");

            if (!TokenMatchesSelectedMode(auth.Token, expectedMode))
                throw new Exception(expectedMode == AppUserMode.Student
                    ? "هذا الحساب ليس حساب طالب. الرجاء اختيار نوع الحساب الصحيح."
                    : "هذا الحساب ليس حساب أهل. الرجاء اختيار نوع الحساب الصحيح.");

            return auth.Token;
        }

        private static bool TokenMatchesSelectedMode(string token, AppUserMode expectedMode)
        {
            var roles = ExtractRoles(token);

            return expectedMode switch
            {
                AppUserMode.Student => HasRole(roles, "Student"),
                AppUserMode.Parent => HasRole(roles, "Parent")
                    || HasRole(roles, "ParentSudent")
                    || HasRole(roles, "ParentStudent"),
                _ => true
            };
        }

        private static IReadOnlyList<string> ExtractRoles(string token)
        {
            var parts = token.Split('.');
            if (parts.Length < 2)
                return Array.Empty<string>();

            var json = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
            var payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
                ?? new Dictionary<string, JsonElement>();

            var roles = new List<string>();
            AddClaimValues(payload, roles, ClaimTypes.Role);
            AddClaimValues(payload, roles, "role");
            AddClaimValues(payload, roles, "roles");

            return roles
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Select(role => role.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static void AddClaimValues(Dictionary<string, JsonElement> payload, List<string> roles, string claimName)
        {
            if (!payload.TryGetValue(claimName, out var claim))
                return;

            if (claim.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in claim.EnumerateArray())
                {
                    var role = item.GetString();
                    if (!string.IsNullOrWhiteSpace(role))
                        roles.Add(role);
                }

                return;
            }

            var singleRole = claim.GetString();
            if (!string.IsNullOrWhiteSpace(singleRole))
                roles.Add(singleRole);
        }

        private static bool HasRole(IReadOnlyList<string> roles, string role)
            => roles.Any(current => string.Equals(current, role, StringComparison.OrdinalIgnoreCase));

        private static byte[] Base64UrlDecode(string value)
        {
            var base64 = value.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}
