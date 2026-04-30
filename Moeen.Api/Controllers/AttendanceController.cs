using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Attendance;
using Moeen.Shared.Responses.Attendance;
using System;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        /// <summary>
        /// تسجيل الحضور اليومي
        /// </summary>
        [HttpPost("record-daily")]
        public async Task<ActionResult<RecordDailyAttendanceResponse>> RecordDailyAttendance(RecordDailyAttendanceRequest request)
            => Ok(await _attendanceService.RecordDailyAttendanceAsync(request));

        /// <summary>
        /// تسجيل الغياب
        /// </summary>
        [HttpPost("record-absence")]
        public async Task<ActionResult<RecordAbsenceResponse>> RecordAbsence(RecordAbsenceRequest request)
            => Ok(await _attendanceService.RecordAbsenceAsync(request));

        /// <summary>
        /// تعديل سجل حضور
        /// </summary>
        [HttpPut("modify-record")]
        public async Task<ActionResult<ModifyAttendanceRecordResponse>> ModifyAttendanceRecord(ModifyAttendanceRecordRequest request)
            => Ok(await _attendanceService.ModifyAttendanceRecordAsync(request));

        /// <summary>
        /// حساب نسبة الحضور لطالب
        /// </summary>
        [HttpPost("calculate-rate")]
        public async Task<ActionResult<CalculateAttendanceRateResponse>> CalculateAttendanceRate(CalculateAttendanceRateRequest request)
            => Ok(await _attendanceService.CalculateAttendanceRateAsync(request));

        /// <summary>
        /// GET (جديد): نسبة حضور الطالب.
        /// </summary>
        [HttpGet("students/{studentId:guid}/rate")]
        public async Task<ActionResult<CalculateAttendanceRateResponse>> CalculateAttendanceRateGet([FromRoute] Guid studentId)
            => Ok(await _attendanceService.CalculateAttendanceRateAsync(new CalculateAttendanceRateRequest { StudentId = studentId }));

        /// <summary>
        /// مراقبة الغياب المتكرر
        /// </summary>
        [HttpPost("frequent-absences")]
        public async Task<ActionResult<MonitorFrequentAbsencesResponse>> MonitorFrequentAbsences(MonitorFrequentAbsencesRequest request)
            => Ok(await _attendanceService.MonitorFrequentAbsencesAsync(request));

        /// <summary>
        /// GET (جديد): مراقبة الغياب المتكرر بعتبة.
        /// </summary>
        [HttpGet("frequent-absences")]
        public async Task<ActionResult<MonitorFrequentAbsencesResponse>> MonitorFrequentAbsencesGet([FromQuery] int threshold = 3)
            => Ok(await _attendanceService.MonitorFrequentAbsencesAsync(new MonitorFrequentAbsencesRequest { Threshold = threshold }));

        /// <summary>
        /// تصدير سجلات الحضور
        /// </summary>
        [HttpPost("export")]
        public async Task<IActionResult> ExportAttendance(ExportAttendanceRequest request)
        {
            var result = await _attendanceService.ExportAttendanceAsync(request);
            return File(result.FileContent, result.ContentType, result.FileName);
        }
    }
}