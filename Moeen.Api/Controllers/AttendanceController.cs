using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Attendance;
using Moeen.Shared.Responses;
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
        public async Task<ActionResult<GeneralResponse>> RecordDailyAttendance([FromBody] RecordDailyAttendanceRequest request)
        {
            var response = await _attendanceService.RecordDailyAttendanceAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// تسجيل الغياب
        /// </summary>
        [HttpPost("record-absence")]
        public async Task<ActionResult<GeneralResponse>> RecordAbsence([FromBody] RecordAbsenceRequest request)
        {
            var response = await _attendanceService.RecordAbsenceAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// تعديل سجل حضور
        /// </summary>
        [HttpPut("modify-record")]
        public async Task<ActionResult<GeneralResponse>> ModifyAttendanceRecord([FromBody] ModifyAttendanceRecordRequest request)
        {
            var response = await _attendanceService.ModifyAttendanceRecordAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// حساب نسبة الحضور لطالب
        /// </summary>
        [HttpPost("calculate-rate")]
        public async Task<ActionResult<GeneralResponse>> CalculateAttendanceRate([FromBody] CalculateAttendanceRateRequest request)
        {
            var response = await _attendanceService.CalculateAttendanceRateAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// GET (جديد): نسبة حضور الطالب.
        /// </summary>
        [HttpGet("students/{studentId:guid}/rate")]
        public async Task<ActionResult<GeneralResponse>> CalculateAttendanceRateGet([FromRoute] Guid studentId)
        {
            var response = await _attendanceService.CalculateAttendanceRateAsync(new CalculateAttendanceRateRequest { StudentId = studentId });
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// مراقبة الغياب المتكرر
        /// </summary>
        [HttpPost("frequent-absences")]
        public async Task<ActionResult<GeneralResponse>> MonitorFrequentAbsences([FromBody] MonitorFrequentAbsencesRequest request)
        {
            var response = await _attendanceService.MonitorFrequentAbsencesAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// GET (جديد): مراقبة الغياب المتكرر بعتبة.
        /// </summary>
        [HttpGet("frequent-absences")]
        public async Task<ActionResult<GeneralResponse>> MonitorFrequentAbsencesGet([FromQuery] int threshold = 3)
        {
            var response = await _attendanceService.MonitorFrequentAbsencesAsync(new MonitorFrequentAbsencesRequest { Threshold = threshold });
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// تصدير سجلات الحضور
        /// </summary>
        [HttpPost("export")]
        public async Task<ActionResult<GeneralResponse>> ExportAttendance([FromBody] ExportAttendanceRequest request)
        {
            var response = await _attendanceService.ExportAttendanceAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}