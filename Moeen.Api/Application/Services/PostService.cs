using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Requests.Post;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Post;

namespace Moeen.Api.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Post CRUD Operations

        public async Task<GeneralResponse> CreatePostAsync(CreatePostRequest createPostRequest)
        {
            try
            {
                var mosque = await _unitOfWork.Repository<Mosque>().GetByIdAsync(createPostRequest.MosqueId);
                if (mosque == null)
                    return GeneralResponse.NotFound("Mosque not found.");

                var post = new Post
                {
                    Id = Guid.NewGuid(),
                    MosqueId = createPostRequest.MosqueId,
                    Title = createPostRequest.Title,
                    Body = createPostRequest.Body,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.Repository<Post>().AddAsync(post);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Post created successfully.", new { post.Id });
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to create post: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> UpdatePostAsync(Guid postId, UpdatePostRequest updatePostRequest)
        {
            try
            {
                var post = await _unitOfWork.Repository<Post>().GetByIdAsync(postId);
                if (post == null)
                    return GeneralResponse.NotFound("Post not found.");

                post.Title = updatePostRequest.Title;
                post.Body = updatePostRequest.Body;

                await _unitOfWork.Repository<Post>().UpdateAsync(post);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Post updated successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to update post: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> DeletePostAsync(Guid postId)
        {
            try
            {
                var post = await _unitOfWork.Repository<Post>().GetByIdAsync(postId);
                if (post == null)
                    return GeneralResponse.NotFound("Post not found.");

                await _unitOfWork.Repository<Post>().DeleteAsync(post);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Post deleted successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to delete post: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetPostByIdAsync(Guid postId)
        {
            try
            {
                var spec = Spec.For<Post>(p => p.Id == postId);
                spec.AddInclude(p => p.Mosque);

                var post = (await _unitOfWork.Repository<Post>().GetAllAsync(spec)).FirstOrDefault();
                if (post == null)
                    return GeneralResponse.NotFound("Post not found.");

                var postResponse = new PostResponse
                {
                    Id = post.Id,
                    MosqueId = post.MosqueId,
                    MosqueName = post.Mosque?.Name,
                    Title = post.Title,
                    Body = post.Body,
                    CreatedAt = post.CreatedAt
                };

                return GeneralResponse.Ok("Post retrieved successfully.", postResponse);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve post: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetAllPostsAsync()
        {
            try
            {
                var spec = Spec.For<Post>(p => true);
                spec.AddInclude(p => p.Mosque);

                var posts = await _unitOfWork.Repository<Post>().GetAllAsync(spec);

                var postsResponse = posts.Select(p => new PostResponse
                {
                    Id = p.Id,
                    MosqueId = p.MosqueId,
                    MosqueName = p.Mosque?.Name,
                    Title = p.Title,
                    Body = p.Body,
                    CreatedAt = p.CreatedAt
                }).ToList();

                return GeneralResponse.Ok("Posts retrieved successfully.", postsResponse);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve posts: {ex.Message}");
            }
        }

        #endregion

        #region Custom Operations

        public async Task<GeneralResponse> GetPostsByMosqueIdAsync(Guid mosqueId)
        {
            try
            {
                var mosque = await _unitOfWork.Repository<Mosque>().GetByIdAsync(mosqueId);
                if (mosque == null)
                    return GeneralResponse.NotFound("Mosque not found.");

                var spec = Spec.For<Post>(p => p.MosqueId == mosqueId);
                spec.AddInclude(p => p.Mosque);

                var posts = await _unitOfWork.Repository<Post>().GetAllAsync(spec);

                var postsResponse = posts.Select(p => new PostResponse
                {
                    Id = p.Id,
                    MosqueId = p.MosqueId,
                    MosqueName = p.Mosque?.Name,
                    Title = p.Title,
                    Body = p.Body,
                    CreatedAt = p.CreatedAt
                }).ToList();

                return GeneralResponse.Ok($"Posts for mosque {mosque.Name} retrieved successfully.", postsResponse);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve mosque posts: {ex.Message}");
            }
        }

        #endregion
    }
}