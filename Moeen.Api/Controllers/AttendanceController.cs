using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Attendance;
using Moeen.Api.Shared.Responses.Attendance;
using System.Threading.Tasks;

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
        {
            var result = await _attendanceService.RecordDailyAttendanceAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تسجيل الغياب
        /// </summary>
        [HttpPost("record-absence")]
        public async Task<ActionResult<RecordAbsenceResponse>> RecordAbsence(RecordAbsenceRequest request)
        {
            var result = await _attendanceService.RecordAbsenceAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تعديل سجل حضور
        /// </summary>
        [HttpPut("modify-record")]
        public async Task<ActionResult<ModifyAttendanceRecordResponse>> ModifyAttendanceRecord(ModifyAttendanceRecordRequest request)
        {
            var result = await _attendanceService.ModifyAttendanceRecordAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// حساب نسبة الحضور لطالب
        /// </summary>
        [HttpPost("calculate-rate")]
        public async Task<ActionResult<CalculateAttendanceRateResponse>> CalculateAttendanceRate(CalculateAttendanceRateRequest request)
        {
            var result = await _attendanceService.CalculateAttendanceRateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// مراقبة الغياب المتكرر
        /// </summary>
        [HttpPost("frequent-absences")]
        public async Task<ActionResult<MonitorFrequentAbsencesResponse>> MonitorFrequentAbsences(MonitorFrequentAbsencesRequest request)
        {
            var result = await _attendanceService.MonitorFrequentAbsencesAsync(request);
            return Ok(result);
        }

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