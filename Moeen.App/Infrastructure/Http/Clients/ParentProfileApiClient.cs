using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class ParentProfileApiClient
    {
        private readonly HttpClient _httpClient;

        public ParentProfileApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetMyProfileAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return GeneralResponse.Unauthorized("يجب تسجيل الدخول كولي أمر قبل عرض الملف الشخصي.");

            using var request = new HttpRequestMessage(HttpMethod.Get, ApiRoutes.ParentProfileMeRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(request);
                return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                       ?? GeneralResponse.BadRequest("تعذر قراءة استجابة ملف ولي الأمر.");
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
                return GeneralResponse.Unauthorized("يجب تسجيل الدخول كولي أمر قبل إرسال الملاحظة.");

            using var request = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.ParentProfileNoteRoute)
            {
                Content = JsonContent.Create(new SubmitParentProfileNoteRequest { Content = content })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(request);
                return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                       ?? GeneralResponse.BadRequest("تعذر قراءة استجابة إرسال الملاحظة.");
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
