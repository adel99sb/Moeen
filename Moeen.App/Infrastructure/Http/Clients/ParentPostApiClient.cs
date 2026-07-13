using Moeen.Shared.Responses;
using System.Net.Http.Headers;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class ParentPostApiClient
    {
        private readonly HttpClient _httpClient;

        public ParentPostApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetMyPostsAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return GeneralResponse.Unauthorized("يجب تسجيل الدخول كولي أمر قبل عرض المنشورات.");

            using var request = new HttpRequestMessage(HttpMethod.Get, ApiRoutes.ParentPostsMeRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await SendAsync(request, "تعذر قراءة استجابة منشورات ولي الأمر.");
        }

        public async Task<GeneralResponse> ToggleLikeAsync(string token, Guid postId)
        {
            if (string.IsNullOrWhiteSpace(token))
                return GeneralResponse.Unauthorized("يجب تسجيل الدخول كولي أمر قبل التفاعل مع المنشور.");

            using var request = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.ParentPostToggleLikeRoute(postId));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await SendAsync(request, "تعذر قراءة استجابة التفاعل مع المنشور.");
        }

        private async Task<GeneralResponse> SendAsync(HttpRequestMessage request, string fallbackMessage)
        {
            try
            {
                var response = await _httpClient.SendAsync(request);
                return await ApiResponseReader.ReadGeneralResponseAsync(response, fallbackMessage);
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
