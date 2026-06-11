using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.Application;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// جلب إحصائيات لوحة التحكم العامة للنظام (خاصة بمدير النظام الإداري)
        /// </summary>
        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                var result = await _dashboardService.GetAdminDashboardAsync();
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving admin dashboard: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب إحصائيات خاصة بمسجد معين (يستهلكها مشرف أو مدير المسجد)
        /// </summary>
        [HttpGet("mosque/{mosqueId}")]
        public async Task<IActionResult> GetMosqueDashboard(Guid mosqueId)
        {
            try
            {
                var result = await _dashboardService.GetMosqueDashboardAsync(mosqueId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving mosque dashboard: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب إحصائيات أداء وإنتاجية المعلم/الشيخ اليومية والعامة
        /// </summary>
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetTeacherDashboard(Guid teacherId)
        {
            try
            {
                var result = await _dashboardService.GetTeacherDashboardAsync(teacherId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving teacher dashboard: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب الملف الإحصائي الخاص بالطالب لمتابعة مستواه، نسبة حضوره، وآخر ما حفظه
        /// </summary>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentDashboard(Guid studentId)
        {
            try
            {
                var result = await _dashboardService.GetStudentDashboardAsync(studentId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving student dashboard: {ex.Message}");
            }
        }
    }
}