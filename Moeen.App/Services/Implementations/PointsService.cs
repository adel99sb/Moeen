using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Points;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class PointsService : IPointsService
    {
        private readonly PointsApiClient _apiClient;

        public PointsService(PointsApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<int> GetStudentPointsAsync(Guid studentId)
        {
            var res = await _apiClient.GetStudentPointsAsync(studentId);
            if (res == null || !res.Success)
                return 0;

            var json = JsonSerializer.Serialize(res.Data);
            var data = JsonSerializer.Deserialize<GetStudentPointsResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return data?.Points ?? 0;
        }

        public async Task<List<StudentPointsBreakdownDto>> GetStudentPointsBreakdownAsync(Guid studentId)
        {
            var res = await _apiClient.GetStudentPointsBreakdownAsync(studentId);
            if (res == null || !res.Success)
                return new List<StudentPointsBreakdownDto>();

            var json = JsonSerializer.Serialize(res.Data);
            var data = JsonSerializer.Deserialize<List<StudentPointsBreakdownDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return data ?? new List<StudentPointsBreakdownDto>();
        }
    }
}
