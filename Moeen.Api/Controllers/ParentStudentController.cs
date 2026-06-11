using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.ParentStudent;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParentStudentController : ControllerBase
    {
        private readonly IParentStudentService _parentStudentService;

        public ParentStudentController(IParentStudentService parentStudentService)
        {
            _parentStudentService = parentStudentService;
        }

        /// <summary>
        /// ربط ولي أمر بطالب معين في النظام
        /// </summary>
        [HttpPost("link")]
        public async Task<IActionResult> LinkParentToStudent([FromBody] LinkParentStudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _parentStudentService.LinkParentToStudentAsync(request);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while linking parent to student: {ex.Message}");
            }
        }

        /// <summary>
        /// إلغاء الربط وحذف العلاقة بين ولي الأمر والطالب
        /// </summary>
        [HttpDelete("unlink/parents/{parentId}/students/{studentId}")]
        public async Task<IActionResult> UnlinkParentFromStudent(Guid parentId, Guid studentId)
        {
            try
            {
                var result = await _parentStudentService.UnlinkParentFromStudentAsync(parentId, studentId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while unlinking: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب قائمة بكافة الطلاب التابعين لولي أمر محدد عبر الـ ParentId
        /// </summary>
        [HttpGet("parents/{parentId}/students")]
        public async Task<IActionResult> GetStudentsByParentId(Guid parentId)
        {
            try
            {
                var result = await _parentStudentService.GetStudentsByParentIdAsync(parentId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving students: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب أولياء الأمور المرتبطين بطالب محدد عبر الـ StudentId
        /// </summary>
        [HttpGet("students/{studentId}/parents")]
        public async Task<IActionResult> GetParentsByStudentId(Guid studentId)
        {
            try
            {
                var result = await _parentStudentService.GetParentsByStudentIdAsync(studentId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving parents: {ex.Message}");
            }
        }
    }
}