using Moeen.Shared.Responses;
using System.Net.Http.Json;
using Moeen.Shared.Requests.ContentSharing;

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
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                       ?? GeneralResponse.BadRequest("تعذر قراءة استجابة المنشورات العامة.");
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
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.InteractWithPostRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
