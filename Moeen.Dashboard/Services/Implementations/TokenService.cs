using Microsoft.JSInterop;
using Moeen.Dashboard.Services.Abstractions;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Moeen.Dashboard.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private const string Key = "auth_token";
        private readonly IJSRuntime _js;
        private string? _cachedToken;

        public TokenService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task Save(string token)
        {
            _cachedToken = token;
            await _js.InvokeVoidAsync("localStorage.setItem", Key, token);
        }

        public async Task<string?> Get()
        {
            if (!string.IsNullOrWhiteSpace(_cachedToken))
                return _cachedToken;

            _cachedToken = await _js.InvokeAsync<string?>("localStorage.getItem", Key);
            return _cachedToken;
        }

        public async Task Clear()
        {
            _cachedToken = null;
            await _js.InvokeVoidAsync("localStorage.removeItem", Key);
        }

        public async Task<DashboardAuthSession> GetSession()
        {
            var token = await Get();
            if (string.IsNullOrWhiteSpace(token))
                return CreateAnonymousSession();

            try
            {
                var payload = ReadJwtPayload(token);
                if (IsExpired(payload))
                {
                    await Clear();
                    return CreateAnonymousSession();
                }

                var roles = ExtractRoles(payload);
                var primaryRole = ResolvePrimaryRole(roles);

                return new DashboardAuthSession(
                    IsAuthenticated: true,
                    Token: token,
                    Roles: roles,
                    PrimaryRole: primaryRole,
                    HomeRoute: GetHomeRoute(primaryRole));
            }
            catch
            {
                await Clear();
                return CreateAnonymousSession();
            }
        }

        private static DashboardAuthSession CreateAnonymousSession()
            => new(false, null, Array.Empty<string>(), null, "/");

        private static Dictionary<string, JsonElement> ReadJwtPayload(string token)
        {
            var parts = token.Split('.');
            if (parts.Length < 2)
                throw new InvalidOperationException("Invalid token format.");

            var json = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
            return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
                   ?? new Dictionary<string, JsonElement>();
        }

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

        private static bool IsExpired(Dictionary<string, JsonElement> payload)
        {
            if (!payload.TryGetValue("exp", out var expClaim))
                return false;

            if (!expClaim.TryGetInt64(out var expSeconds))
                return false;

            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expSeconds);
            return expiresAt <= DateTimeOffset.UtcNow;
        }

        private static IReadOnlyList<string> ExtractRoles(Dictionary<string, JsonElement> payload)
        {
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

        private static string? ResolvePrimaryRole(IReadOnlyList<string> roles)
        {
            if (HasRole(roles, "Owner")) return "owner";
            if (HasRole(roles, "Admin") || HasRole(roles, "Supervisor")) return "supervisor";
            if (HasRole(roles, "Teacher")) return "teacher";
            if (HasRole(roles, "Examer") || HasRole(roles, "Examiner")) return "examer";

            return roles.FirstOrDefault()?.ToLowerInvariant();
        }

        private static bool HasRole(IReadOnlyList<string> roles, string role)
            => roles.Any(current => string.Equals(current, role, StringComparison.OrdinalIgnoreCase));

        private static string GetHomeRoute(string? primaryRole)
            => primaryRole switch
            {
                "owner" => "/owner/dashboard",
                "supervisor" => "/Supervisor/SupervisorDashboard",
                "teacher" => "/Teacher/TeacherDashboard",
                "examer" => "/Examer/Dashboard",
                _ => "/settings"
            };
    }
}
