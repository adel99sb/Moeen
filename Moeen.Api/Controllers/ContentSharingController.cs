using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.ContentSharing;
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
        public async Task<ActionResult<PostDto>> PublishPost([FromBody] PublishPostRequest request)
            => Ok(await _contentService.PublishPostAsync(request));

        /// <summary>
        /// أمر: التفاعل مع منشور.
        /// </summary>
        [HttpPost("interact")]
        public async Task<ActionResult<InteractWithPostResponse>> InteractWithPost([FromBody] InteractWithPostRequest request)
            => Ok(await _contentService.InteractWithPostAsync(request));

        /// <summary>
        /// POST (قديم/متوافق): بحث متقدم بالمحتوى.
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<SearchContentResponse>> SearchContent([FromBody] SearchContentRequest request)
            => Ok(await _contentService.SearchContentAsync(request));

        /// <summary>
        /// GET (جديد): بحث سريع عبر Query.
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<SearchContentResponse>> SearchContentGet([FromQuery] string query)
            => Ok(await _contentService.SearchContentAsync(new SearchContentRequest { Query = query }));

        /// <summary>
        /// أمر: أرشفة المحتوى القديم.
        /// </summary>
        [HttpPost("archive-old")]
        public async Task<ActionResult<ArchiveOldContentResponse>> ArchiveOldContent([FromBody] ArchiveOldContentRequest request)
            => Ok(await _contentService.ArchiveOldContentAsync(request));

        /// <summary>
        /// أمر: إدارة الإعلانات.
        /// </summary>
        [HttpPost("manage-announcement")]
        public async Task<ActionResult<ManageAnnouncementResponse>> ManageAnnouncement([FromBody] ManageAnnouncementRequest request)
            => Ok(await _contentService.ManageAnnouncementsAsync(request));

        /// <summary>
        /// أمر: إضافة وسائط متعددة لمنشور.
        /// </summary>
        [HttpPost("add-multimedia")]
        public async Task<ActionResult<AddMultimediaResponse>> AddMultimedia([FromBody] AddMultimediaRequest request)
            => Ok(await _contentService.AddMultimediaAsync(request));

        /// <summary>
        /// GET: جلب منشور محدد مع تفاصيل التفاعلات (اختياري).
        /// </summary>
        [HttpGet("posts/{postId:guid}")]
        public async Task<ActionResult<PostDto>> GetPostById(
            [FromRoute] Guid postId,
            [FromQuery] bool includeInteractions = true)
        {
            var request = new GetPostByIdRequest
            {
                PostId = postId,
                IncludeInteractions = includeInteractions
            };

            return Ok(await _contentService.GetPostByIdAsync(request));
        }

        /// <summary>
        /// GET: جلب قائمة التفاعلات على منشور.
        /// </summary>
        [HttpGet("posts/{postId:guid}/interactions")]
        public async Task<ActionResult<List<InteractionDto>>> GetPostInteractions(
            [FromRoute] Guid postId,
            [FromQuery] GetPostInteractionsRequest request)
        {
            request.PostId = postId;
            return Ok(await _contentService.GetPostInteractionsAsync(request));
        }

        /// <summary>
        /// PUT: تحديث منشور موجود.
        /// </summary>
        [HttpPut("posts/{postId:guid}")]
        public async Task<ActionResult<PostDto>> UpdatePost(
            [FromRoute] Guid postId,
            [FromBody] UpdatePostRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.PostId = postId;
            return Ok(await _contentService.UpdatePostAsync(request));
        }

        /// <summary>
        /// DELETE: حذف منشور.
        /// </summary>
        [HttpDelete("posts/{postId:guid}")]
        public async Task<ActionResult<OperationResponseDto>> DeletePost([FromRoute] Guid postId)
            => Ok(await _contentService.DeletePostAsync(new DeletePostRequest { PostId = postId }));
    }
}