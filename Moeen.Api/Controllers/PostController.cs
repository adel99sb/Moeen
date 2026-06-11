using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Post;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        #region Post CRUD Operations

        /// <summary>
        /// إنشاء منشور أو إعلان جديد في النظام تابع لمسجد معين
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest createPostRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _postService.CreatePostAsync(createPostRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while creating post: {ex.Message}");
            }
        }

        /// <summary>
        /// تعديل بيانات منشور معين بواسطة المعرف الرقمي (Guid)
        /// </summary>
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostRequest updatePostRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _postService.UpdatePostAsync(postId, updatePostRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating post: {ex.Message}");
            }
        }

        /// <summary>
        /// حذف منشور نهائياً من النظام
        /// </summary>
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            try
            {
                var result = await _postService.DeletePostAsync(postId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting post: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب بيانات منشور محدد عن طريق الـ Id
        /// </summary>
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPostById(Guid postId)
        {
            try
            {
                var result = await _postService.GetPostByIdAsync(postId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving post: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب قائمة بكافة المنشورات المسجلة في النظام لجميع المساجد
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var result = await _postService.GetAllPostsAsync();
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving posts: {ex.Message}");
            }
        }

        #endregion

        #region Custom Operations

        /// <summary>
        /// جلب قائمة بكافة المنشورات التابعة لجامع محدد بواسطة الـ MosqueId
        /// </summary>
        [HttpGet("mosque/{mosqueId}")]
        public async Task<IActionResult> GetPostsByMosqueId(Guid mosqueId)
        {
            try
            {
                var result = await _postService.GetPostsByMosqueIdAsync(mosqueId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving posts for this mosque: {ex.Message}");
            }
        }

        #endregion
    }
}