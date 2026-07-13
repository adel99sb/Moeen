using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContentSharingController : ControllerBase
    {
        private readonly IContentSharingService _contentService;

        public ContentSharingController(IContentSharingService contentService)
        {
            _contentService = contentService;
        }

        [HttpPost("publish")]
        [Authorize(Roles = "Admin,Owner,Supervisor")]
        public async Task<ActionResult<GeneralResponse>> PublishPost([FromBody] PublishPostRequest request)
        {
            var result = await _contentService.PublishPostAsync(request);
            return ToActionResult(result);
        }

        [HttpGet("halqas/brief")]
        public async Task<ActionResult<GeneralResponse>> GetAvailableHalqasBrief([FromQuery] string? query)
        {
            var result = await _contentService.GetAvailableHalqasBriefAsync(query);
            return ToActionResult(result);
        }

        [HttpPost("interact")]
        public async Task<ActionResult<GeneralResponse>> InteractWithPost([FromBody] InteractWithPostRequest request)
        {
            var result = await _contentService.InteractWithPostAsync(request);
            return ToActionResult(result);
        }

        [HttpPost("search")]
        public async Task<ActionResult<GeneralResponse>> SearchContent([FromBody] SearchContentRequest request)
        {
            var result = await _contentService.SearchContentAsync(request);
            return ToActionResult(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<GeneralResponse>> SearchContentGet([FromQuery] string query)
        {
            var result = await _contentService.SearchContentAsync(new SearchContentRequest { Query = query });
            return ToActionResult(result);
        }

        [HttpPost("delete-old")]
        public async Task<ActionResult<GeneralResponse>> DeleteOldContent([FromBody] DeleteOldContentRequest request)
        {
            var result = await _contentService.DeleteOldContentAsync(request);
            return ToActionResult(result);
        }

        [HttpPost("manage-announcement")]
        public async Task<ActionResult<GeneralResponse>> ManageAnnouncement([FromBody] ManageAnnouncementRequest request)
        {
            var result = await _contentService.ManageAnnouncementsAsync(request);
            return ToActionResult(result);
        }

        [HttpPost("add-multimedia")]
        public async Task<ActionResult<GeneralResponse>> AddMultimedia([FromBody] AddMultimediaRequest request)
        {
            var result = await _contentService.AddMultimediaAsync(request);
            return ToActionResult(result);
        }

        [HttpGet("posts/{postId:guid}")]
        public async Task<ActionResult<GeneralResponse>> GetPostById(
            [FromRoute] Guid postId,
            [FromQuery] bool includeInteractions = true)
        {
            var result = await _contentService.GetPostByIdAsync(new GetPostByIdRequest
            {
                PostId = postId,
                IncludeInteractions = includeInteractions
            });

            return ToActionResult(result);
        }

        [HttpGet("posts/{postId:guid}/interactions")]
        public async Task<ActionResult<GeneralResponse>> GetPostInteractions(
            [FromRoute] Guid postId,
            [FromQuery] GetPostInteractionsRequest request)
        {
            request.PostId = postId;
            var result = await _contentService.GetPostInteractionsAsync(request);
            return ToActionResult(result);
        }

        [HttpPut("posts/{postId:guid}")]
        [Authorize(Roles = "Admin,Owner,Supervisor")]
        public async Task<ActionResult<GeneralResponse>> UpdatePost(
            [FromRoute] Guid postId,
            [FromBody] UpdatePostRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(GeneralResponse.BadRequest("Invalid request.", ModelState));

            request.PostId = postId;
            var result = await _contentService.UpdatePostAsync(request);
            return ToActionResult(result);
        }

        [HttpDelete("posts/{postId:guid}")]
        [Authorize(Roles = "Admin,Owner,Supervisor")]
        public async Task<ActionResult<GeneralResponse>> DeletePost([FromRoute] Guid postId)
        {
            var result = await _contentService.DeletePostAsync(new DeletePostRequest { PostId = postId });
            return ToActionResult(result);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<GeneralResponse>> GetAllPosts()
        {
            var result = await _contentService.GetAllPostsAsync();
            return ToActionResult(result);
        }

        private ActionResult<GeneralResponse> ToActionResult(GeneralResponse? result)
        {
            if (result == null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    GeneralResponse.InternalError("Content sharing operation failed."));

            return StatusCode(result.StatusCode, result);
        }
    }
}

