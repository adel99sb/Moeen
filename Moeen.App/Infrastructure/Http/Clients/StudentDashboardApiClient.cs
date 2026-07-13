using Moeen.Shared.Responses;
using System.Net.Http.Headers;

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
                return await ApiResponseReader.ReadGeneralResponseAsync(
                    response,
                    "تعذر قراءة استجابة الصفحة الرئيسية.");
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
