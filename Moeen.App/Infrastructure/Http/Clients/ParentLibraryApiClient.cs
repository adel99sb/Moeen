using Moeen.Shared.Responses;
using System.Globalization;
using System.Net.Http.Headers;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class ParentLibraryApiClient
    {
        private readonly HttpClient _httpClient;

        public ParentLibraryApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetBooksAsync(string token, int pageNumber, int pageSize, string? query)
        {
            if (string.IsNullOrWhiteSpace(token))
                return GeneralResponse.Unauthorized("يجب تسجيل الدخول كولي أمر قبل عرض المكتبة.");

            using var request = new HttpRequestMessage(HttpMethod.Get, BuildRoute(pageNumber, pageSize, query));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(request);
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

        private static string BuildRoute(int pageNumber, int pageSize, string? query)
        {
            var parts = new List<string>
            {
                $"pageNumber={Math.Max(pageNumber, 1).ToString(CultureInfo.InvariantCulture)}",
                $"pageSize={Math.Max(pageSize, 1).ToString(CultureInfo.InvariantCulture)}"
            };

            if (!string.IsNullOrWhiteSpace(query))
                parts.Add($"query={Uri.EscapeDataString(query.Trim())}");

            return $"{ApiRoutes.ParentLibraryBooksRoute}?{string.Join("&", parts)}";
        }
    }
}
