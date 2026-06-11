using Moeen.Shared.Responses.Attendance;

namespace Moeen.App.Services.Abstractions
{
    public interface IAttendanceService
    {
        Task<AttendanceRateDto> GetStudentAttendanceRateAsync(Guid studentId);
        Task<StudentAbsenceReportDto> GetStudentAbsenceReportAsync(Guid studentId);
    }
}
