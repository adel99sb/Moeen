using Moeen.Shared.Requests;
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

        // ================= LOGIN =================

        public async Task<GeneralResponse> Login(LoginRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.LoginRoute, request);

            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // ================= REGISTER =================

        public async Task<GeneralResponse> Register(RegisterRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.RegisterRoute, request);

            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // ================= USERS =================

        public async Task<GeneralResponse> SearchUsers(
            PaginationRequest request,
            string? keyword = null)
        {
            string url = ApiRoutes.SearchUsersRoute;

            if (!string.IsNullOrWhiteSpace(keyword))
                url += $"?keyword={Uri.EscapeDataString(keyword)}";

            var response = await _http.PostAsJsonAsync(url, request);

            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetUserById(Guid id)
        {
            return await _http.GetFromJsonAsync<GeneralResponse>(
                ApiRoutes.GetUserById(id));
        }

        // ================= EMAIL =================

        public async Task<GeneralResponse> ChangeEmail(string email)
        {
            var response = await _http.PutAsJsonAsync(
                ApiRoutes.ChangeEmailRoute,
                email);

            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // ================= PASSWORD =================

        public async Task<GeneralResponse> SendResetUrl(
            SendPasswordResetUrlRequest request)
        {
            var response = await _http.PostAsJsonAsync(
                ApiRoutes.SendResetUrlRoute,
                request);

            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> ResetPassword(
            ChangePasswordRequest request)
        {
            var response = await _http.PostAsJsonAsync(
                ApiRoutes.ResetPasswordRoute,
                request);

            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // ================= VERIFICATION =================

        public async Task<GeneralResponse> SendVerifyCode(
            SendVerifyEmailCodeRequest request)
        {
            var response = await _http.PostAsJsonAsync(
                ApiRoutes.SendVerifyCodeRoute,
                request);

            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> VerifyEmail(
            VerifyEmailRequest request)
        {
            var response = await _http.PostAsJsonAsync(
                ApiRoutes.VerifyEmailRoute,
                request);

            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}