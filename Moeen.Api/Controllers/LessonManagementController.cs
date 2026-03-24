using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.LessonManagement;
using Moeen.Api.Shared.Responses.LessonManagement;
using System.Threading.Tasks;

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

        [HttpPost("create")]
        public async Task<ActionResult<LessonDto>> CreateLesson(CreateLessonRequest request)
        {
            var result = await _lessonService.CreateLessonAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<ActionResult<LessonDto>> UpdateLesson(UpdateLessonRequest request)
        {
            var result = await _lessonService.UpdateLessonAsync(request);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<DeleteLessonResponse>> DeleteLesson(DeleteLessonRequest request)
        {
            var result = await _lessonService.DeleteLessonAsync(request);
            return Ok(result);
        }

        [HttpPost("reorder")]
        public async Task<ActionResult<ReorderLessonsResponse>> ReorderLessons(ReorderLessonsRequest request)
        {
            var result = await _lessonService.ReorderLessonsAsync(request);
            return Ok(result);
        }

        [HttpPost("add-materials")]
        public async Task<ActionResult<AddLessonMaterialsResponse>> AddLessonMaterials(AddLessonMaterialsRequest request)
        {
            var result = await _lessonService.AddLessonMaterialsAsync(request);
            return Ok(result);
        }

        [HttpPost("manage-time")]
        public async Task<ActionResult<ManageLessonTimeResponse>> ManageLessonTime(ManageLessonTimeRequest request)
        {
            var result = await _lessonService.ManageLessonTimeAsync(request);
            return Ok(result);
        }

        [HttpPost("copy")]
        public async Task<ActionResult<CopyLessonsResponse>> CopyLessons(CopyLessonsRequest request)
        {
            var result = await _lessonService.CopyLessonsAsync(request);
            return Ok(result);
        }
    }
}