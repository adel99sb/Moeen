using Moeen.Shared.Responses;

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
                return await ApiResponseReader.ReadGeneralResponseAsync(
                    response,
                    "تعذر قراءة استجابة المكتبة.");
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
