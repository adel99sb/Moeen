using Moeen.Shared.Responses;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class StudentDashboardApiClient
    {
        private readonly HttpClient _httpClient;

        public StudentDashboardApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetMyDashboardAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return GeneralResponse.Unauthorized("يجب تسجيل الدخول قبل عرض الصفحة الرئيسية.");

            using var request = new HttpRequestMessage(HttpMethod.Get, ApiRoutes.StudentDashboardMeRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(request);
                var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();

                if (result != null)
                    return result;

                return response.IsSuccessStatusCode
                    ? GeneralResponse.Ok("تم جلب بيانات الصفحة الرئيسية.")
                    : GeneralResponse.BadRequest("تعذر قراءة استجابة السيرفر.");
            }
            catch (HttpRequestException)
            {
                return GeneralResponse.BadRequest("تعذر الاتصال بالسيرفر. تأكد أن الـ API يعمل وأن عنوان الشبكة صحيح.");
            }
            catch (TaskCanceledException)
            {
                return GeneralResponse.BadRequest("انتهت مهلة الاتصال بالسيرفر.");
            }
        }
    }
}
