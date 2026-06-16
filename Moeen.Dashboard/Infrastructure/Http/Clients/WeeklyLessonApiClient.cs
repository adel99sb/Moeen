using System.Net.Http.Json;
using Moeen.Shared.Requests.LessonManagement;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class WeeklyLessonApiClient
    {
        private readonly HttpClient _httpClient;

        public WeeklyLessonApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetWeeklyLessonsAsync()
        {
            return await _httpClient.GetFromJsonAsync<GeneralResponse>("api/lesson-management/weekly-lessons")
                ?? GeneralResponse.BadRequest("فشل تحميل الدروس الأسبوعية.");
        }

        public async Task<GeneralResponse> CreateWeeklyLessonAsync(CreateWeeklyLessonRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/lesson-management/weekly-lessons", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                ?? GeneralResponse.BadRequest("فشل إنشاء الدرس الأسبوعي.");
        }

        public async Task<GeneralResponse> UpdateWeeklyLessonAsync(Guid lessonId, UpdateWeeklyLessonRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/lesson-management/weekly-lessons/{lessonId}", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                ?? GeneralResponse.BadRequest("فشل تعديل الدرس الأسبوعي.");
        }

        public async Task<GeneralResponse> DeleteWeeklyLessonAsync(Guid lessonId)
        {
            var response = await _httpClient.DeleteAsync($"api/lesson-management/weekly-lessons/{lessonId}");
            return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                ?? GeneralResponse.BadRequest("فشل حذف الدرس الأسبوعي.");
        }

        public async Task<GeneralResponse> CreateWeeklyLessonRowAsync(Guid lessonId, CreateWeeklyLessonAssignmentRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/lesson-management/weekly-lessons/{lessonId}/rows", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                ?? GeneralResponse.BadRequest("فشل إضافة موعد الحلقة.");
        }

        public async Task<GeneralResponse> UpdateWeeklyLessonRowAsync(Guid rowId, UpdateWeeklyLessonAssignmentRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/lesson-management/weekly-lesson-rows/{rowId}", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                ?? GeneralResponse.BadRequest("فشل تعديل موعد الحلقة.");
        }

        public async Task<GeneralResponse> DeleteWeeklyLessonRowAsync(Guid rowId)
        {
            var response = await _httpClient.DeleteAsync($"api/lesson-management/weekly-lesson-rows/{rowId}");
            return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                ?? GeneralResponse.BadRequest("فشل حذف موعد الحلقة.");
        }
    }
}
