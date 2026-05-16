using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Identity;
using System.Text.Json;
using System.Net.Http.Json;
namespace Moeen.Dashboard.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AuthApiClient _client;
        private object _Client;

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
        public async Task<GeneralResponse> Register(RegisterRequest request)
        {
            // 1. منبعث الطلب عن طريق الـ client (ساعي البريد)
            var res = await _client.Register(request);

            // 2. التحقق إذا النتيجة رجعت فاضية أو فيها فشل
            if (res == null || !res.Success)
            {
                // منطلع خطأ أو منرجع رسالة الفشل
                throw new Exception(res?.Message ?? "Registration failed");
            }

            // 3. تحويل البيانات (مثل ما عملتي بالـ Login تماماً)
            var json = JsonSerializer.Serialize(res.Data);
            var data = JsonSerializer.Deserialize<GeneralResponse>(json);

            return data;
        }
        public async Task<GeneralResponse> Search(SearchMembersRequest request)
        {
            // 1. منبعث الطلب عن طريق الـ client (ساعي البريد)
            var res = await _client.Search(request);

            // 2. التحقق إذا النتيجة رجعت فاضية أو فيها فشل
            if (res == null || !res.Success)
            {
                // منطلع خطأ أو منرجع رسالة الفشل
                throw new Exception(res?.Message ?? "Search failed");
            }

            return res;
        }
        public async Task<GeneralResponse> SendVerifyEmailCode(SendVerifyEmailCodeRequest request)
        {
            return await _client.SendVerifyEmailCode(request);
        }
        public async Task<GeneralResponse> VerifyEmail(VerifyEmailRequest request)
        {
            return await _client.VerifyEmail(request);
        }
        public async Task<GeneralResponse> GetPostByIdAsync(Guid postId)
        {
            // 1. منبعث الطلب عن طريق الـ client (ساعي البريد)
            var res = await _client.GetPostById(postId);

            // 2. التحقق إذا النتيجة رجعت فاضية أو فيها فشل
            if (res == null || !res.Success)
            {
                throw new Exception(res?.Message ?? "Failed to fetch post");
            }

            // 3. إرجاع النتيجة مباشرة متل ميثود الـ Search تماماً
            return res;
        }
        public async Task<GeneralResponse> GetPostInteractionsAsync(Guid postId)
        {
            // مننادي ساعي البريد (الكلاينت) اللي جهزناه بالخطوة الأولى
            var res = await _client.GetPostInteractionsAsync(postId);

            // لو النتيجة رجعت فاضية أو فيها فشل، منرمي Exception يوضح المشكلة
            if (res == null || !res.Success)
            {
                throw new Exception(res?.Message ?? "فشلت عملية جلب التفاعلات");
            }

            // لو كل شي تمام، منرجع التفاعلات للشاشة
            return res;
        }
        
    }
}
