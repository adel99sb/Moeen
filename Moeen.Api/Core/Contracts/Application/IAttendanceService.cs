using Moeen.Api.Shared.Requests.Attendance;
using Moeen.Api.Shared.Responses.Attendance;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IAttendanceService
    {
        /// <summary>
        /// تسجيل الحضور اليومي
        /// </summary>
        Task<RecordDailyAttendanceResponse> RecordDailyAttendanceAsync(RecordDailyAttendanceRequest request);

        /// <summary>
        /// تسجيل الغياب بعذر أو بدون
        /// </summary>
        Task<RecordAbsenceResponse> RecordAbsenceAsync(RecordAbsenceRequest request);

        /// <summary>
        /// تعديل سجلات الحضور
        /// </summary>
        Task<ModifyAttendanceRecordResponse> ModifyAttendanceRecordAsync(ModifyAttendanceRecordRequest request);

        /// <summary>
        /// حساب نسب الحضور
        /// </summary>
        Task<CalculateAttendanceRateResponse> CalculateAttendanceRateAsync(CalculateAttendanceRateRequest request);

        /// <summary>
        /// الغياب المتكرر
        /// </summary>
        Task<MonitorFrequentAbsencesResponse> MonitorFrequentAbsencesAsync(MonitorFrequentAbsencesRequest request);

        /// <summary>
        /// تصدير سجلات الحضور
        /// </summary>
        Task<ExportAttendanceResponse> ExportAttendanceAsync(ExportAttendanceRequest request);
    }
}