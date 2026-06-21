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
    [Authorize(Roles = nameof(Roles.Student))]
    [Route("api/mobile/student-posts")]
    public class MobileStudentPostsController : ControllerBase
    {
        private readonly IStudentMobilePostService _postService;
        private readonly ICurrentUserService _currentUserService;

        public MobileStudentPostsController(
            IStudentMobilePostService postService,
            ICurrentUserService currentUserService)
        {
            _postService = postService;
            _currentUserService = currentUserService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<GeneralResponse>> GetMyPosts()
        {
            var studentId = _currentUserService.CurrentUserId;
            if (!studentId.HasValue)
                return Unauthorized(GeneralResponse.Unauthorized("لم يتم التعرف على الطالب الحالي."));

            var response = await _postService.GetPostsAsync(studentId.Value);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("{postId:guid}/toggle-like")]
        public async Task<ActionResult<GeneralResponse>> ToggleLike([FromRoute] Guid postId)
        {
            var studentId = _currentUserService.CurrentUserId;
            if (!studentId.HasValue)
                return Unauthorized(GeneralResponse.Unauthorized("لم يتم التعرف على الطالب الحالي."));

            var response = await _postService.ToggleLikeAsync(studentId.Value, postId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
