using System.Text.Json;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.LessonManagement;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.LessonManagement;

namespace Moeen.Dashboard.Services.Implementations
{
    public class WeeklyLessonService : IWeeklyLessonService
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
        private readonly WeeklyLessonApiClient _apiClient;

        public WeeklyLessonService(WeeklyLessonApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<WeeklyLessonManagementDto>> GetWeeklyLessonsAsync()
        {
            var response = await _apiClient.GetWeeklyLessonsAsync();
            EnsureSuccess(response);
            return DeserializeData<List<WeeklyLessonManagementDto>>(response.Data) ?? new List<WeeklyLessonManagementDto>();
        }

        public Task<GeneralResponse> CreateWeeklyLessonAsync(CreateWeeklyLessonRequest request)
            => _apiClient.CreateWeeklyLessonAsync(request);

        public Task<GeneralResponse> UpdateWeeklyLessonAsync(Guid lessonId, UpdateWeeklyLessonRequest request)
            => _apiClient.UpdateWeeklyLessonAsync(lessonId, request);

        public Task<GeneralResponse> DeleteWeeklyLessonAsync(Guid lessonId)
            => _apiClient.DeleteWeeklyLessonAsync(lessonId);

        public Task<GeneralResponse> CreateWeeklyLessonRowAsync(Guid lessonId, CreateWeeklyLessonAssignmentRequest request)
            => _apiClient.CreateWeeklyLessonRowAsync(lessonId, request);

        public Task<GeneralResponse> UpdateWeeklyLessonRowAsync(Guid rowId, UpdateWeeklyLessonAssignmentRequest request)
            => _apiClient.UpdateWeeklyLessonRowAsync(rowId, request);

        public Task<GeneralResponse> DeleteWeeklyLessonRowAsync(Guid rowId)
            => _apiClient.DeleteWeeklyLessonRowAsync(rowId);

        private static void EnsureSuccess(GeneralResponse? response)
        {
            if (response == null || !response.Success)
                throw new InvalidOperationException(response?.Message ?? "Request failed.");
        }

        private static T? DeserializeData<T>(object? data)
        {
            if (data == null)
                return default;

            var json = JsonSerializer.Serialize(data);
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
    }
}
