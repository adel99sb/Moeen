using Moeen.Shared.Requests.Attendance;
using Moeen.Shared.Responses.Analytics;
using Moeen.Shared.Responses.Attendance;
using System;

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

        /// <summary>
        /// استعلام عن سجل حضور محدد بواسطة معرفه
        /// </summary>
        Task<AttendanceRecordDto> GetAttendanceRecordByIdAsync(Guid recordId);

        /// <summary>
        /// الحصول على قائمة سجلات الحضور مع إمكانية الفلترة والترقيم
        /// </summary>
        Task<PagedResult<AttendanceRecordSummaryDto>> GetAllAttendanceRecordsAsync(AttendanceRecordFilter filter);

        /// <summary>
        /// الحصول على نسب حضور طالب معين في فترة زمنية
        /// </summary>
        Task<AttendanceRateDto> GetStudentAttendanceRateAsync(Guid studentId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// الحصول على قائمة الطلاب المتكرر غيابهم
        /// </summary>
        Task<FrequentAbsencesResultDto> GetFrequentAbsencesAsync(FrequentAbsencesFilter filter);

        /// <summary>
        /// حذف سجل حضور أو غياب
        /// </summary>
        Task<bool> DeleteAttendanceRecordAsync(Guid recordId);

        /// <summary>
        /// تحديث حالة حضور لسجل موجود
        /// </summary>
        Task<AttendanceRecordDto> UpdateAttendanceStatusAsync(Guid recordId, UpdateAttendanceStatusRequest request);
    }
}