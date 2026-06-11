using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Exam;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        #region Exam CRUD Operations

        /// <summary>
        /// تسجيل نتيجة اختبار جديد لطالب (من جزء إلى جزء مع الدرجة) بواسطة الشيخ/المعلم
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] CreateExamRequest createRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _examService.CreateExamAsync(createRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while creating exam record: {ex.Message}");
            }
        }

        /// <summary>
        /// تعديل بيانات اختبار مسجلة مسبقاً (الأجزاء أو العلامة)
        /// </summary>
        [HttpPut("{examId}")]
        public async Task<IActionResult> UpdateExam(Guid examId, [FromBody] UpdateExamRequest updateRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _examService.UpdateExamAsync(examId, updateRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating exam record: {ex.Message}");
            }
        }

        /// <summary>
        /// حذف سجل اختبار معين من النظام نهائياً
        /// </summary>
        [HttpDelete("{examId}")]
        public async Task<IActionResult> DeleteExam(Guid examId)
        {
            try
            {
                var result = await _examService.DeleteExamAsync(examId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting exam record: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب تفاصيل اختبار محدد بواسطة الـ Id
        /// </summary>
        [HttpGet("{examId}")]
        public async Task<IActionResult> GetExamById(Guid examId)
        {
            try
            {
                var result = await _examService.GetExamByIdAsync(examId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving exam record: {ex.Message}");
            }
        }

        #endregion

        #region Custom Operations

        /// <summary>
        /// جلب جميع الاختبارات التي خاضها طالب محدد للوقوف على مستوى درجاته
        /// </summary>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentExams(Guid studentId)
        {
            try
            {
                var result = await _examService.GetStudentExamsAsync(studentId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving student exams: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب الاختبارات التي أشرف عليها معلم معين في تاريخ محدد (لمتابعة إنتاجية المعلم)
        /// </summary>
        /// <param name="teacherId">معرف المعلم الرقمي</param>
        /// <param name="date">التاريخ المراد البحث فيه (YYYY-MM-DD)، وفي حال تركه فارغاً يتم اعتماد تاريخ اليوم</param>
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetTeacherExamsByDate(Guid teacherId, [FromQuery] DateTime? date)
        {
            try
            {
                var result = await _examService.GetExamsByTeacherAndDateAsync(teacherId, date);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving teacher exam logs: {ex.Message}");
            }
        }

        #endregion
    }
}