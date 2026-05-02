using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ContentSharing;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.ContentSharing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentSharingController : ControllerBase
    {
        private readonly IContentSharingService _contentService;

        public ContentSharingController(IContentSharingService contentService)
        {
            _contentService = contentService;
        }

        /// <summary>
        /// أمر: نشر منشور جديد.
        /// </summary>
        [HttpPost("publish")]
        public async Task<ActionResult<GeneralResponse>> PublishPost([FromBody] PublishPostRequest request)
        {
            var result = await _contentService.PublishPostAsync(request);
            if (result == null)
                return NotFound(GeneralResponse.NotFound("Publish failed."));
            return Ok(GeneralResponse.Ok("Post published.", result));
        }

        /// <summary>
        /// أمر: التفاعل مع منشور.
        /// </summary>
        [HttpPost("interact")]
        public async Task<ActionResult<GeneralResponse>> InteractWithPost([FromBody] InteractWithPostRequest request)
        {
            var result = await _contentService.InteractWithPostAsync(request);
            if (result == null)
                return Problem("Interaction failed.", statusCode: 500);
            if (result.Success)
                return Ok(GeneralResponse.Ok(result.Message, result));
            return BadRequest(GeneralResponse.BadRequest(result.Message, result));
        }

        /// <summary>
        /// POST (قديم/متوافق): بحث متقدم بالمحتوى.
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<GeneralResponse>> SearchContent([FromBody] SearchContentRequest request)
        {
            var result = await _contentService.SearchContentAsync(request);
            if (result == null)
                return Ok(GeneralResponse.Ok("No results.", new SearchContentResponse { Posts = new List<PostDto>() }));
            return Ok(GeneralResponse.Ok("Search results.", result));
        }

        /// <summary>
        /// GET (جديد): بحث سريع عبر Query.
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<GeneralResponse>> SearchContentGet([FromQuery] string query)
        {
            var result = await _contentService.SearchContentAsync(new SearchContentRequest { Query = query });
            if (result == null)
                return Ok(GeneralResponse.Ok("No results.", new SearchContentResponse { Posts = new List<PostDto>() }));
            return Ok(GeneralResponse.Ok("Search results.", result));
        }

        /// <summary>
        /// أمر: أرشفة المحتوى القديم.
        /// </summary>
        [HttpPost("delete-old")]
        public async Task<ActionResult<GeneralResponse>> DeleteOldContent([FromBody] DeleteOldContentRequest request)
        {
            var result = await _contentService.DeleteOldContentAsync(request);
            if (result == null)
                return Problem("Delete old content failed.", statusCode: 500);
            if (result.Success)
                return Ok(GeneralResponse.Ok(result.Message, result));
            return BadRequest(GeneralResponse.BadRequest(result.Message, result));
        }

        /// <summary>
        /// أمر: إدارة الإعلانات.
        /// </summary>
        [HttpPost("manage-announcement")]
        public async Task<ActionResult<GeneralResponse>> ManageAnnouncement([FromBody] ManageAnnouncementRequest request)
        {
            var result = await _contentService.ManageAnnouncementsAsync(request);
            if (result == null)
                return Problem("Manage announcement failed.", statusCode: 500);
            if (result.Success)
                return Ok(GeneralResponse.Ok(result.Message, result));
            return BadRequest(GeneralResponse.BadRequest(result.Message, result));
        }

        /// <summary>
        /// أمر: إضافة وسائط متعددة لمنشور.
        /// </summary>
        [HttpPost("add-multimedia")]
        public async Task<ActionResult<GeneralResponse>> AddMultimedia([FromBody] AddMultimediaRequest request)
        {
            var result = await _contentService.AddMultimediaAsync(request);
            if (result == null)
                return Problem("Add multimedia failed.", statusCode: 500);
            if (result.Success)
                return Ok(GeneralResponse.Ok(result.Message, result));
            return BadRequest(GeneralResponse.BadRequest(result.Message, result));
        }

        /// <summary>
        /// GET: جلب منشور محدد مع تفاصيل التفاعلات (اختياري).
        /// </summary>
        [HttpGet("posts/{postId:guid}")]
        public async Task<ActionResult<GeneralResponse>> GetPostById(
            [FromRoute] Guid postId,
            [FromQuery] bool includeInteractions = true)
        {
            var request = new GetPostByIdRequest
            {
                PostId = postId,
                IncludeInteractions = includeInteractions
            };

            var result = await _contentService.GetPostByIdAsync(request);
            if (result == null)
                return NotFound(GeneralResponse.NotFound("Post not found."));
            return Ok(GeneralResponse.Ok("Post retrieved.", result));
        }

        /// <summary>
        /// GET: جلب قائمة التفاعلات على منشور.
        /// </summary>
        [HttpGet("posts/{postId:guid}/interactions")]
        public async Task<ActionResult<GeneralResponse>> GetPostInteractions(
            [FromRoute] Guid postId,
            [FromQuery] GetPostInteractionsRequest request)
        {
            request.PostId = postId;
            var result = await _contentService.GetPostInteractionsAsync(request);
            return Ok(GeneralResponse.Ok("Interactions retrieved.", result));
        }

        /// <summary>
        /// PUT: تحديث منشور موجود.
        /// </summary>
        [HttpPut("posts/{postId:guid}")]
        public async Task<ActionResult<GeneralResponse>> UpdatePost(
            [FromRoute] Guid postId,
            [FromBody] UpdatePostRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(GeneralResponse.BadRequest("Invalid request.", ModelState));

            request.PostId = postId;
            var result = await _contentService.UpdatePostAsync(request);
            if (result == null)
                return NotFound(GeneralResponse.NotFound("Post not found or update failed."));
            return Ok(GeneralResponse.Ok("Post updated.", result));
        }

        /// <summary>
        /// DELETE: حذف منشور.
        /// </summary>
        [HttpDelete("posts/{postId:guid}")]
        public async Task<ActionResult<GeneralResponse>> DeletePost([FromRoute] Guid postId)
        {
            var result = await _contentService.DeletePostAsync(new DeletePostRequest { PostId = postId });
            if (result == null)
                return Problem("Delete failed.", statusCode: 500);
            if (result.Success)
                return Ok(GeneralResponse.Ok(result.Message, result));
            return BadRequest(GeneralResponse.BadRequest(result.Message, result));
        }
    }
}