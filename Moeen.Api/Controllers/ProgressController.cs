using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Progress;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        #region Progress CRUD Operations

        /// <summary>
        /// تسجيل قيد تقدم وإنجاز جديد لطالب (حفظ أجزاء وصفحات) بواسطة المعلم
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProgressEntry([FromBody] CreateProgressEntryRequest createRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _progressService.CreateProgressEntryAsync(createRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while creating progress entry: {ex.Message}");
            }
        }

        /// <summary>
        /// تعديل الجزء أو الصفحة المسجلة بالخطأ في قيد إنجاز معين
        /// </summary>
        [HttpPut("{entryId}")]
        public async Task<IActionResult> UpdateProgressEntry(Guid entryId, [FromBody] UpdateProgressEntryRequest updateRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _progressService.UpdateProgressEntryAsync(entryId, updateRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating progress entry: {ex.Message}");
            }
        }

        /// <summary>
        /// حذف سجل إنجاز من النظام نهائياً
        /// </summary>
        [HttpDelete("{entryId}")]
        public async Task<IActionResult> DeleteProgressEntry(Guid entryId)
        {
            try
            {
                var result = await _progressService.DeleteProgressEntryAsync(entryId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting progress entry: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب تفاصيل قيد إنجاز محدد بواسطة الـ Id
        /// </summary>
        [HttpGet("{entryId}")]
        public async Task<IActionResult> GetProgressEntryById(Guid entryId)
        {
            try
            {
                var result = await _progressService.GetProgressEntryByIdAsync(entryId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving progress entry: {ex.Message}");
            }
        }

        #endregion

        #region Custom Operations

        /// <summary>
        /// جلب السجل التاريخي الكامل لتقدم طالب محدد (لمتابعة خطة حفظه)
        /// </summary>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentProgressHistory(Guid studentId)
        {
            try
            {
                var result = await _progressService.GetStudentProgressHistoryAsync(studentId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving student progress history: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب جميع القيود التي سجلها معلم محدد في تاريخ معين (للتدقيق اليومي)
        /// </summary>
        /// <param name="teacherId">معرف المعلم الرقمي</param>
        /// <param name="date">التاريخ المراد البحث فيه (YYYY-MM-DD)، وإذا ترك فارغاً يجلب تاريخ اليوم</param>
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetTeacherEntriesByDate(Guid teacherId, [FromQuery] DateTime? date)
        {
            try
            {
                var result = await _progressService.GetTeacherEntriesByDateAsync(teacherId, date);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving teacher progress entries: {ex.Message}");
            }
        }

        #endregion
    }
}