using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Responses;
using System.Net;
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
                var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();

                if (result != null)
                    return result;

                return BuildFailureResponse(response.StatusCode, "رجع السيرفر رداً فارغاً أثناء تسجيل الدخول.");
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

        private static GeneralResponse BuildFailureResponse(HttpStatusCode statusCode, string message)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => GeneralResponse.BadRequest(message),
                HttpStatusCode.Unauthorized => GeneralResponse.Unauthorized(message),
                HttpStatusCode.NotFound => GeneralResponse.NotFound(message),
                >= HttpStatusCode.InternalServerError => GeneralResponse.InternalError(message),
                _ => new GeneralResponse(message, false, (int)statusCode)
            };
        }
    }
}
