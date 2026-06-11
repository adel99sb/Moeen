using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Attendance;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly AttendanceApiClient _apiClient;

        public AttendanceService(AttendanceApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<AttendanceRateDto> GetStudentAttendanceRateAsync(Guid studentId)
        {
            var res = await _apiClient.GetStudentAttendanceRateAsync(studentId);
            if (res == null || !res.Success)
                return new AttendanceRateDto();

            var json = JsonSerializer.Serialize(res.Data);
            var data = JsonSerializer.Deserialize<AttendanceRateDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return data ?? new AttendanceRateDto();
        }

        public async Task<StudentAbsenceReportDto> GetStudentAbsenceReportAsync(Guid studentId)
        {
            var res = await _apiClient.GetStudentAbsenceReportAsync(studentId);
            if (res == null || !res.Success)
                return new StudentAbsenceReportDto();

            var json = JsonSerializer.Serialize(res.Data);
            var data = JsonSerializer.Deserialize<StudentAbsenceReportDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return data ?? new StudentAbsenceReportDto();
        }
    }
}
