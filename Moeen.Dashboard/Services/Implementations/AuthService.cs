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

        public async Task<GeneralResponse> Register(RegisterRequest request)
        {
            var res = await _client.CreateUser(request);

            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Unknown error");

            return res;
        }

        // تم إضافة ميثود السيرش ليتطابق مع الـ Interface
        public async Task<GeneralResponse> Search(SearchMembersRequest request)
        {
            var res = await _client.Search(request);
            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Unknown error");

            return res;
        }

        // تم إضافة ميثود إرسال كود التفعيل
        public async Task<GeneralResponse> SendVerifyEmailCode(SendVerifyEmailCodeRequest request)
        {
            return await _client.SendVerifyEmailCode(request);
        }

        // تم إضافة ميثود التحقق من الإيميل
        public async Task<GeneralResponse> VerifyEmail(VerifyEmailRequest request)
        {
            return await _client.VerifyEmail(request);
        }

        public async Task<GeneralResponse> GetPostByIdAsync(Guid postId)
        {
            var res = await _client.GetPostById(postId);

            if (res == null || !res.Success)
            {
                throw new Exception(res?.Message ?? "Failed to fetch post");
            }

            return res;
        }

        public async Task<GeneralResponse> GetPostInteractionsAsync(Guid postId)
        {
            var res = await _client.GetPostInteractionsAsync(postId);

            if (res == null || !res.Success)
            {
                throw new Exception(res?.Message ?? "فشلت عملية جلب التفاعلات");
            }

            return res;
        }

        public async Task<GeneralResponse> DeletePostAsync(Guid postId)
        {
            return await _client.DeletePostAsync(postId);
        }
    }
}