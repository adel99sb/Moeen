using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using System.Globalization;
using System.Net.Http.Headers;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class ParentProgressApiClient
    {
        private readonly HttpClient _httpClient;

        public ParentProgressApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetMyChildProgressAsync(
            string token,
            Guid? childId,
            DateTime? from,
            DateTime? to,
            ProgressRecordType? type)
        {
            if (string.IsNullOrWhiteSpace(token))
                return GeneralResponse.Unauthorized("يجب تسجيل الدخول كولي أمر قبل عرض تقدم الطالب.");

            using var request = new HttpRequestMessage(HttpMethod.Get, BuildRoute(childId, from, to, type));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(request);
                return await ApiResponseReader.ReadGeneralResponseAsync(
                    response,
                    "تعذر قراءة استجابة تقدم الطالب.");
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

        private static string BuildRoute(Guid? childId, DateTime? from, DateTime? to, ProgressRecordType? type)
        {
            var query = new List<string>();

            if (childId.HasValue && childId.Value != Guid.Empty)
                query.Add($"childId={Uri.EscapeDataString(childId.Value.ToString())}");

            if (from.HasValue)
                query.Add($"from={Uri.EscapeDataString(from.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");

            if (to.HasValue)
                query.Add($"to={Uri.EscapeDataString(to.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");

            if (type.HasValue)
                query.Add($"type={(int)type.Value}");

            return query.Count == 0
                ? ApiRoutes.ParentProgressMeRoute
                : $"{ApiRoutes.ParentProgressMeRoute}?{string.Join("&", query)}";
        }
    }
}
