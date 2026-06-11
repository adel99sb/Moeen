using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.DailyAssignments;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class DailyAssignmentsService : IDailyAssignmentsService
    {
        private readonly DailyAssignmentsApiClient _apiClient;

        public DailyAssignmentsService(DailyAssignmentsApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<DailyAssignmentDto>> GetDailyAssignmentsAsync(Guid studentId, DateTime? date = null, Guid? halqaId = null)
        {
            var res = await _apiClient.GetByStudentAsync(studentId, date, halqaId);
            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Unknown error");

            var json = JsonSerializer.Serialize(res.Data);
            var data = JsonSerializer.Deserialize<List<DailyAssignmentDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return data ?? new List<DailyAssignmentDto>();
        }
    }
}
