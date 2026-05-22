using Moeen.Shared.Requests.Attendance;
using Moeen.Shared.Responses;
using System;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IAttendanceService
    {
        /// <summary>
        /// تسجيل الحضور اليومي
        /// </summary>
        Task<GeneralResponse> RecordDailyAttendanceAsync(RecordDailyAttendanceRequest request);

        /// <summary>
        /// تسجيل الغياب بعذر أو بدون
        /// </summary>
        Task<GeneralResponse> RecordAbsenceAsync(RecordAbsenceRequest request);

        /// <summary>
        /// تعديل سجلات الحضور
        /// </summary>
        Task<GeneralResponse> ModifyAttendanceRecordAsync(ModifyAttendanceRecordRequest request);

        /// <summary>
        /// حساب نسب الحضور
        /// </summary>
        Task<GeneralResponse> CalculateAttendanceRateAsync(CalculateAttendanceRateRequest request);

        /// <summary>
        /// الغياب المتكرر
        /// </summary>
        Task<GeneralResponse> MonitorFrequentAbsencesAsync(MonitorFrequentAbsencesRequest request);

        /// <summary>
        /// تصدير سجلات الحضور
        /// </summary>
        Task<GeneralResponse> ExportAttendanceAsync(ExportAttendanceRequest request);

        /// <summary>
        /// استعلام عن سجل حضور محدد بواسطة معرفه
        /// </summary>
        Task<GeneralResponse> GetAttendanceRecordByIdAsync(Guid recordId);

        /// <summary>
        /// الحصول على قائمة سجلات الحضور مع إمكانية الفلترة والترقيم
        /// </summary>
        Task<GeneralResponse> GetAllAttendanceRecordsAsync(AttendanceRecordFilter filter);

        /// <summary>
        /// الحصول على نسب حضور طالب معين في فترة زمنية
        /// </summary>
        Task<GeneralResponse> GetStudentAttendanceRateAsync(Guid studentId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// الحصول على قائمة الطلاب المتكرر غيابهم
        /// </summary>
        Task<GeneralResponse> GetFrequentAbsencesAsync(FrequentAbsencesFilter filter);

        /// <summary>
        /// حذف سجل حضور أو غياب
        /// </summary>
        Task<GeneralResponse> DeleteAttendanceRecordAsync(Guid recordId);

        /// <summary>
        /// تحديث حالة حضور لسجل موجود
        /// </summary>
        Task<GeneralResponse> UpdateAttendanceStatusAsync(Guid recordId, UpdateAttendanceStatusRequest request);

        /// <summary>
        /// تسجيل حضور المعلم
        /// </summary>
        Task<GeneralResponse> RecordTeacherAttendanceAsync(RecordTeacherAttendanceRequest request);

        /// <summary>
        /// حساب نسبة حضور المعلم
        /// </summary>
        Task<GeneralResponse> GetTeacherAttendanceRateAsync(GetTeacherAttendanceRateRequest request);
    }
}