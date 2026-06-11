using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Attendance;

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

        #region Attendance CRUD Operations

        /// <summary>
        /// تسجيل حضور/غياب جديد لطالب في المسجد لليوم المحدد
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RecordAttendance([FromBody] CreateAttendanceRequest createAttendanceRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _attendanceService.RecordAttendanceAsync(createAttendanceRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while recording attendance: {ex.Message}");
            }
        }

        /// <summary>
        /// تعديل حالة حضور/غياب مسجلة مسبقاً (مثلاً تعديل من غائب إلى حاضر أو إضافة ملاحظة)
        /// </summary>
        [HttpPut("{attendanceId}")]
        public async Task<IActionResult> UpdateAttendance(Guid attendanceId, [FromBody] UpdateAttendanceRequest updateAttendanceRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _attendanceService.UpdateAttendanceAsync(attendanceId, updateAttendanceRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating attendance record: {ex.Message}");
            }
        }

        /// <summary>
        /// حذف سجل تحضير معين نهائياً من النظام
        /// </summary>
        [HttpDelete("{attendanceId}")]
        public async Task<IActionResult> DeleteAttendance(Guid attendanceId)
        {
            try
            {
                var result = await _attendanceService.DeleteAttendanceAsync(attendanceId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting attendance record: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب تفاصيل سجل تحضير محدد عبر الـ Id الخاص به
        /// </summary>
        [HttpGet("{attendanceId}")]
        public async Task<IActionResult> GetAttendanceById(Guid attendanceId)
        {
            try
            {
                var result = await _attendanceService.GetAttendanceByIdAsync(attendanceId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving attendance record: {ex.Message}");
            }
        }

        #endregion

        #region Custom Operations

        /// <summary>
        /// جلب سجل الحضور والغياب الكامل لطالب محدد عبر الـ StudentId
        /// </summary>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentAttendanceHistory(Guid studentId)
        {
            try
            {
                var result = await _attendanceService.GetStudentAttendanceHistoryAsync(studentId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving student attendance history: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب كشف الحضور والغياب لكافة طلاب مسجد معين في تاريخ محدد
        /// </summary>
        /// <param name="mosqueId">المعرف الفريد للمسجد</param>
        /// <param name="date">التاريخ المراد جلب الكشف له بصيغة YYYY-MM-DD</param>
        [HttpGet("mosque/{mosqueId}")]
        public async Task<IActionResult> GetMosqueAttendanceByDate(Guid mosqueId, [FromQuery] DateTime? date)
        {
            try
            {
                var result = await _attendanceService.GetMosqueAttendanceByDateAsync(mosqueId, date);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving mosque attendance: {ex.Message}");
            }
        }

        #endregion
    }
}