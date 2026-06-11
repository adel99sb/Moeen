using Moeen.Shared.Requests.Post;
using Moeen.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IPostService
    {
        Task<GeneralResponse> CreatePostAsync(CreatePostRequest createPostRequest);
        Task<GeneralResponse> UpdatePostAsync(Guid postId, UpdatePostRequest updatePostRequest);
        Task<GeneralResponse> DeletePostAsync(Guid postId);
        Task<GeneralResponse> GetPostByIdAsync(Guid postId);
        Task<GeneralResponse> GetAllPostsAsync();

        Task<GeneralResponse> GetPostsByMosqueIdAsync(Guid mosqueId);
    }
}
