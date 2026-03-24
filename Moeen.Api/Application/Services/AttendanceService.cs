using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Attendance;
using Moeen.Api.Shared.Responses.Attendance;

namespace Moeen.Api.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        public Task<CalculateAttendanceRateResponse> CalculateAttendanceRateAsync(CalculateAttendanceRateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExportAttendanceResponse> ExportAttendanceAsync(ExportAttendanceRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ModifyAttendanceRecordResponse> ModifyAttendanceRecordAsync(ModifyAttendanceRecordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MonitorFrequentAbsencesResponse> MonitorFrequentAbsencesAsync(MonitorFrequentAbsencesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RecordAbsenceResponse> RecordAbsenceAsync(RecordAbsenceRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RecordDailyAttendanceResponse> RecordDailyAttendanceAsync(RecordDailyAttendanceRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
