using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = nameof(Roles.ParentSudent))]
    [Route("api/mobile/parent-posts")]
    public class MobileParentPostsController : ControllerBase
    {
        private readonly IParentMobilePostService _postService;
        private readonly ICurrentUserService _currentUserService;

        public MobileParentPostsController(
            IParentMobilePostService postService,
            ICurrentUserService currentUserService)
        {
            _postService = postService;
            _currentUserService = currentUserService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<GeneralResponse>> GetMyPosts()
        {
            var parentId = _currentUserService.CurrentUserId;
            if (!parentId.HasValue)
                return Unauthorized(GeneralResponse.Unauthorized("لم يتم التعرف على ولي الأمر الحالي."));

            var response = await _postService.GetPostsAsync(parentId.Value);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("{postId:guid}/toggle-like")]
        public async Task<ActionResult<GeneralResponse>> ToggleLike([FromRoute] Guid postId)
        {
            var parentId = _currentUserService.CurrentUserId;
            if (!parentId.HasValue)
                return Unauthorized(GeneralResponse.Unauthorized("لم يتم التعرف على ولي الأمر الحالي."));

            var response = await _postService.ToggleLikeAsync(parentId.Value, postId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
