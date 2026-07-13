using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class AuthApiClient
    {
        private readonly HttpClient _httpClient;

        public AuthApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiRoutes.LoginRoute, request);
                return await ApiResponseReader.ReadGeneralResponseAsync(
                    response,
                    "تعذر قراءة استجابة تسجيل الدخول.");
            }
            catch (HttpRequestException)
            {
                return GeneralResponse.InternalError("تعذر الاتصال بسيرفر الـ API. تأكد من تشغيل السيرفر ورابط الاتصال.");
            }
            catch (TaskCanceledException)
            {
                return GeneralResponse.InternalError("انتهت مهلة الاتصال أثناء تسجيل الدخول. حاول مرة ثانية.");
            }
        }
    }
}
