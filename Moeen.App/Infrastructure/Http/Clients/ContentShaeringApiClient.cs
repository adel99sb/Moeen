using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class ContentShaeringApiClient
    {
        private readonly HttpClient _httpClient;

        public ContentShaeringApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiRoutes.GetAllContentSharingRoute);
                return await ApiResponseReader.ReadGeneralResponseAsync(
                    response,
                    "تعذر قراءة استجابة المنشورات العامة.");
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

        public async Task<GeneralResponse?> InteractAsync(InteractWithPostRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiRoutes.InteractWithPostRoute, request);
                return await ApiResponseReader.ReadGeneralResponseAsync(
                    response,
                    "تعذر قراءة استجابة التفاعل مع المنشور.");
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
