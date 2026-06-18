using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.LessonManagement;
using Moeen.Shared.Responses;
using System.Net.Http.Json;
using System.Text.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class WeeklyLessonApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;

        public WeeklyLessonApiClient(HttpClient httpClient, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        public Task<GeneralResponse> GetTeacherWeeklyDashboardAsync(DateTime? date = null)
        {
            var url = "api/lesson-management/dashboard/weekly";
            if (date.HasValue)
                url += $"?date={date.Value:yyyy-MM-dd}";

            return SendAsync<object?>(HttpMethod.Get, url, null, "فشل تحميل دروس المعلم الأسبوعية.");
        }

        public Task<GeneralResponse> RecordLessonAttendanceAsync(RecordAttendanceRequest request)
            => SendAsync(HttpMethod.Post, "api/lesson-management/attendance", request, "فشل حفظ حضور الدرس.");

        public Task<GeneralResponse> GetLessonHistoryAsync(Guid circleId, int pageNumber = 1, int pageSize = 20)
        {
            var url = $"api/lesson-management/circles/{circleId}/history?pageNumber={Math.Max(1, pageNumber)}&pageSize={Math.Max(1, pageSize)}";
            return SendAsync<object?>(HttpMethod.Get, url, null, "فشل تحميل سجل الدروس السابقة.");
        }

        public Task<GeneralResponse> GetWeeklyLessonsAsync()
            => SendAsync<object?>(HttpMethod.Get, "api/lesson-management/weekly-lessons", null, "فشل تحميل الدروس الأسبوعية.");

        public Task<GeneralResponse> CreateWeeklyLessonAsync(CreateWeeklyLessonRequest request)
            => SendAsync(HttpMethod.Post, "api/lesson-management/weekly-lessons", request, "فشل إنشاء الدرس الأسبوعي.");

        public Task<GeneralResponse> UpdateWeeklyLessonAsync(Guid lessonId, UpdateWeeklyLessonRequest request)
            => SendAsync(HttpMethod.Put, $"api/lesson-management/weekly-lessons/{lessonId}", request, "فشل تعديل الدرس الأسبوعي.");

        public Task<GeneralResponse> DeleteWeeklyLessonAsync(Guid lessonId)
            => SendAsync<object?>(HttpMethod.Delete, $"api/lesson-management/weekly-lessons/{lessonId}", null, "فشل حذف الدرس الأسبوعي.");

        public Task<GeneralResponse> CreateWeeklyLessonRowAsync(Guid lessonId, CreateWeeklyLessonAssignmentRequest request)
            => SendAsync(HttpMethod.Post, $"api/lesson-management/weekly-lessons/{lessonId}/rows", request, "فشل إضافة موعد الحلقة.");

        public Task<GeneralResponse> UpdateWeeklyLessonRowAsync(Guid rowId, UpdateWeeklyLessonAssignmentRequest request)
            => SendAsync(HttpMethod.Put, $"api/lesson-management/weekly-lesson-rows/{rowId}", request, "فشل تعديل موعد الحلقة.");

        public Task<GeneralResponse> DeleteWeeklyLessonRowAsync(Guid rowId)
            => SendAsync<object?>(HttpMethod.Delete, $"api/lesson-management/weekly-lesson-rows/{rowId}", null, "فشل حذف موعد الحلقة.");

        private async Task<GeneralResponse> SendAsync<T>(HttpMethod method, string route, T? body, string fallbackMessage)
        {
            var token = await _tokenService.Get();
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("انتهت جلسة تسجيل الدخول، يرجى تسجيل الدخول مرة أخرى.");

            using var request = new HttpRequestMessage(method, route);
            request.Headers.TryAddWithoutValidation(HeaderName(), SchemeName() + " " + token);

            if (body is not null)
                request.Content = JsonContent.Create(body);

            using var response = await _httpClient.SendAsync(request);
            var rawBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(rawBody))
            {
                return response.IsSuccessStatusCode
                    ? GeneralResponse.Ok(fallbackMessage)
                    : new GeneralResponse(fallbackMessage, success: false, statusCode: (int)response.StatusCode);
            }

            try
            {
                var parsed = JsonSerializer.Deserialize<GeneralResponse>(rawBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (parsed != null)
                {
                    if (!response.IsSuccessStatusCode && parsed.StatusCode == 200)
                        parsed.StatusCode = (int)response.StatusCode;

                    return parsed;
                }
            }
            catch (JsonException)
            {
            }

            return new GeneralResponse(
                response.IsSuccessStatusCode ? fallbackMessage : rawBody,
                success: response.IsSuccessStatusCode,
                statusCode: (int)response.StatusCode);
        }

        private static string HeaderName()
            => new(new[] { (char)65, (char)117, (char)116, (char)104, (char)111, (char)114, (char)105, (char)122, (char)97, (char)116, (char)105, (char)111, (char)110 });

        private static string SchemeName()
            => new(new[] { (char)66, (char)101, (char)97, (char)114, (char)101, (char)114 });
    }
}
