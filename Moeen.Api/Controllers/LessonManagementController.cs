using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.LessonManagement;
using Moeen.Api.Shared.Responses.LessonManagement;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonManagementController : ControllerBase
    {
        private readonly ILessonManagementService _lessonService;

        public LessonManagementController(ILessonManagementService lessonService)
        {
            _lessonService = lessonService;
        }

        /// <summary>
        /// أمر: إنشاء درس جديد.
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<LessonDto>> CreateLesson(CreateLessonRequest request)
            => Ok(await _lessonService.CreateLessonAsync(request));

        /// <summary>
        /// أمر: تحديث بيانات الدرس.
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<LessonDto>> UpdateLesson(UpdateLessonRequest request)
            => Ok(await _lessonService.UpdateLessonAsync(request));

        /// <summary>
        /// أمر: حذف درس.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<ActionResult<DeleteLessonResponse>> DeleteLesson(DeleteLessonRequest request)
            => Ok(await _lessonService.DeleteLessonAsync(request));

        /// <summary>
        /// أمر: إعادة ترتيب الدروس.
        /// </summary>
        [HttpPost("reorder")]
        public async Task<ActionResult<ReorderLessonsResponse>> ReorderLessons(ReorderLessonsRequest request)
            => Ok(await _lessonService.ReorderLessonsAsync(request));

        /// <summary>
        /// أمر: إضافة مواد للدرس.
        /// </summary>
        [HttpPost("add-materials")]
        public async Task<ActionResult<AddLessonMaterialsResponse>> AddLessonMaterials(AddLessonMaterialsRequest request)
            => Ok(await _lessonService.AddLessonMaterialsAsync(request));

        /// <summary>
        /// أمر: إدارة وقت الدرس.
        /// </summary>
        [HttpPost("manage-time")]
        public async Task<ActionResult<ManageLessonTimeResponse>> ManageLessonTime(ManageLessonTimeRequest request)
            => Ok(await _lessonService.ManageLessonTimeAsync(request));

        /// <summary>
        /// أمر: نسخ الدروس.
        /// </summary>
        [HttpPost("copy")]
        public async Task<ActionResult<CopyLessonsResponse>> CopyLessons(CopyLessonsRequest request)
            => Ok(await _lessonService.CopyLessonsAsync(request));
    }
}   