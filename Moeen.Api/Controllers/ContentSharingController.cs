using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ContentSharing;
using Moeen.Api.Shared.Responses.ContentSharing;
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

        [HttpPost("publish")]
        public async Task<ActionResult<PostDto>> PublishPost(PublishPostRequest request)
        {
            var result = await _contentService.PublishPostAsync(request);
            return Ok(result);
        }

        [HttpPost("interact")]
        public async Task<ActionResult<InteractWithPostResponse>> InteractWithPost(InteractWithPostRequest request)
        {
            var result = await _contentService.InteractWithPostAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// POST (قديم/متوافق): بحث متقدم/حالي.
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<SearchContentResponse>> SearchContent(SearchContentRequest request)
        {
            var result = await _contentService.SearchContentAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// GET (جديد): بحث سريع عبر Query String.
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<SearchContentResponse>> SearchContentGet([FromQuery] string query)
        {
            var request = new SearchContentRequest { Query = query };
            var result = await _contentService.SearchContentAsync(request);
            return Ok(result);
        }

        [HttpPost("archive-old")]
        public async Task<ActionResult<ArchiveOldContentResponse>> ArchiveOldContent(ArchiveOldContentRequest request)
        {
            var result = await _contentService.ArchiveOldContentAsync(request);
            return Ok(result);
        }

        [HttpPost("manage-announcement")]
        public async Task<ActionResult<ManageAnnouncementResponse>> ManageAnnouncement(ManageAnnouncementRequest request)
        {
            var result = await _contentService.ManageAnnouncementsAsync(request);
            return Ok(result);
        }

        [HttpPost("add-multimedia")]
        public async Task<ActionResult<AddMultimediaResponse>> AddMultimedia(AddMultimediaRequest request)
        {
            var result = await _contentService.AddMultimediaAsync(request);
            return Ok(result);
        }
    }
}