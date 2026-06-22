using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class StudentProfileApiClient
    {
        private readonly HttpClient _httpClient;

        public StudentProfileApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetMyProfileAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return GeneralResponse.Unauthorized("يجب تسجيل الدخول قبل عرض الملف الشخصي.");

            using var request = new HttpRequestMessage(HttpMethod.Get, ApiRoutes.StudentProfileMeRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(request);
                return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                       ?? GeneralResponse.BadRequest("تعذر قراءة استجابة السيرفر.");
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

        public async Task<GeneralResponse> SubmitNoteAsync(string token, string content)
        {
            if (string.IsNullOrWhiteSpace(token))
                return GeneralResponse.Unauthorized("يجب تسجيل الدخول قبل إرسال الملاحظة.");

            using var request = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.StudentProfileNoteRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(new SubmitStudentProfileNoteRequest { Content = content });

            try
            {
                var response = await _httpClient.SendAsync(request);
                return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                       ?? GeneralResponse.BadRequest("تعذر قراءة استجابة السيرفر.");
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
