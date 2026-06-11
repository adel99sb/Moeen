using Moeen.Shared.Requests.Attendance;
using Moeen.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IAttendanceService
    {
        Task<GeneralResponse> RecordAttendanceAsync(CreateAttendanceRequest createAttendanceRequest);
        Task<GeneralResponse> UpdateAttendanceAsync(Guid attendanceId, UpdateAttendanceRequest updateAttendanceRequest);
        Task<GeneralResponse> DeleteAttendanceAsync(Guid attendanceId);
        Task<GeneralResponse> GetAttendanceByIdAsync(Guid attendanceId);

        Task<GeneralResponse> GetStudentAttendanceHistoryAsync(Guid studentId);
        Task<GeneralResponse> GetMosqueAttendanceByDateAsync(Guid mosqueId, DateTime? date);
    }
}