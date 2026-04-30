using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Attendance;
using Moeen.Shared.Responses.Analytics;
using Moeen.Shared.Responses.Attendance;

namespace Moeen.Api.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        public Task<CalculateAttendanceRateResponse> CalculateAttendanceRateAsync(CalculateAttendanceRateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAttendanceRecordAsync(Guid recordId)
        {
            throw new NotImplementedException();
        }

        public Task<ExportAttendanceResponse> ExportAttendanceAsync(ExportAttendanceRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AttendanceRecordSummaryDto>> GetAllAttendanceRecordsAsync(AttendanceRecordFilter filter)
        {
            throw new NotImplementedException();
        }

        public Task<AttendanceRecordDto> GetAttendanceRecordByIdAsync(Guid recordId)
        {
            throw new NotImplementedException();
        }

        public Task<FrequentAbsencesResultDto> GetFrequentAbsencesAsync(FrequentAbsencesFilter filter)
        {
            throw new NotImplementedException();
        }

        public Task<AttendanceRateDto> GetStudentAttendanceRateAsync(Guid studentId, DateTime fromDate, DateTime toDate)
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

        public Task<AttendanceRecordDto> UpdateAttendanceStatusAsync(Guid recordId, UpdateAttendanceStatusRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
