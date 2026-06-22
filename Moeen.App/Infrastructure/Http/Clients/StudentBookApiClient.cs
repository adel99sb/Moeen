using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class StudentBookApiClient
    {
        private readonly HttpClient _httpClient;

        public StudentBookApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetBooksAsync(int pageNumber = 1, int pageSize = 100)
        {
            try
            {
                var route = $"{ApiRoutes.LibraryBooksRoute}?pageNumber={pageNumber}&pageSize={pageSize}";
                var response = await _httpClient.GetAsync(route);
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
