using Moeen.Shared.Responses.Mobile;

namespace Moeen.App.Services.Abstractions
{
    public interface IStudentPostService
    {
        Task<List<StudentPostDto>> GetMyPostsAsync();
        Task<StudentPostInteractionResponse> ToggleLikeAsync(Guid postId);
    }
}
