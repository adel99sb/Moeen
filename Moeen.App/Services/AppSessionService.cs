using Microsoft.Maui.Storage;
using System.Text;
using System.Text.Json;

namespace Moeen.App.Services
{
    public sealed class AppSessionService
    {
        private const string AuthTokenKey = "moeen.mobile.authToken";
        private const string UserModeKey = "moeen.mobile.userMode";

        private bool _initialized;

        public event Action? Changed;

        public AppUserMode? CurrentMode { get; private set; }
        public string? AuthToken { get; private set; }
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AuthToken);
        public bool IsInitialized => _initialized;

        public async Task InitializeAsync()
        {
            if (_initialized)
                return;

            _initialized = true;

            try
            {
                var token = await SecureStorage.Default.GetAsync(AuthTokenKey);
                var modeValue = Preferences.Default.Get(UserModeKey, string.Empty);

                if (string.IsNullOrWhiteSpace(token) || !Enum.TryParse<AppUserMode>(modeValue, out var mode))
                {
                    ClearInMemory();
                    return;
                }

                if (IsJwtExpired(token))
                {
                    await ClearStoredSessionAsync();
                    ClearInMemory();
                    return;
                }

                CurrentMode = mode;
                AuthToken = token;
                NotifyChanged();
            }
            catch
            {
                await ClearStoredSessionAsync();
                ClearInMemory();
            }
        }

        public void ContinueAsGuest()
        {
            CurrentMode = AppUserMode.Guest;
            AuthToken = null;
            _ = ClearStoredSessionAsync();
            NotifyChanged();
        }

        public async Task SignInAsync(AppUserMode mode, string token)
        {
            if (mode is not (AppUserMode.Student or AppUserMode.Parent))
                throw new InvalidOperationException("يمكن حفظ جلسة الطالب أو ولي الأمر فقط.");

            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("لا يمكن حفظ جلسة بدون JWT token.");

            CurrentMode = mode;
            AuthToken = token;
            _initialized = true;

            await SecureStorage.Default.SetAsync(AuthTokenKey, token);
            Preferences.Default.Set(UserModeKey, mode.ToString());

            NotifyChanged();
        }

        public void SignIn(AppUserMode mode, string token)
        {
            CurrentMode = mode;
            AuthToken = token;
            _initialized = true;
            NotifyChanged();
        }

        public async Task LogoutAsync()
        {
            await ClearAsync();
        }

        public async Task ClearAsync()
        {
            await ClearStoredSessionAsync();
            ClearInMemory();
            NotifyChanged();
        }

        public void Clear()
        {
            _ = ClearAsync();
        }

        public string GetHomeRoute()
        {
            return GetHomeRoute(CurrentMode ?? AppUserMode.Guest);
        }

        public static string GetHomeRoute(AppUserMode mode)
        {
            return mode switch
            {
                AppUserMode.Student => "/Student/StudentDashboard",
                AppUserMode.Parent => "/Parent/Parentdashboard",
                _ => "/Guest/Post"
            };
        }

        private static async Task ClearStoredSessionAsync()
        {
            try
            {
                SecureStorage.Default.Remove(AuthTokenKey);
                Preferences.Default.Remove(UserModeKey);
                await Task.CompletedTask;
            }
            catch
            {
                Preferences.Default.Remove(UserModeKey);
            }
        }

        private void ClearInMemory()
        {
            CurrentMode = null;
            AuthToken = null;
        }

        private static bool IsJwtExpired(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length < 2)
                    return true;

                var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
                using var document = JsonDocument.Parse(payloadJson);

                if (!document.RootElement.TryGetProperty("exp", out var expElement))
                    return false;

                var expUnixSeconds = expElement.ValueKind switch
                {
                    JsonValueKind.Number when expElement.TryGetInt64(out var number) => number,
                    JsonValueKind.String when long.TryParse(expElement.GetString(), out var textNumber) => textNumber,
                    _ => 0
                };

                if (expUnixSeconds <= 0)
                    return true;

                var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expUnixSeconds);
                return expiresAt <= DateTimeOffset.UtcNow;
            }
            catch
            {
                return true;
            }
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

        private void NotifyChanged()
        {
            Changed?.Invoke();
        }
    }
}
