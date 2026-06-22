using Moeen.Shared.Responses.Mobile;

namespace Moeen.App.Services.Abstractions
{
    public interface IParentPostService
    {
        Task<List<ParentPostDto>> GetMyPostsAsync();
        Task<ParentPostInteractionResponse> ToggleLikeAsync(Guid postId);
    }
}
