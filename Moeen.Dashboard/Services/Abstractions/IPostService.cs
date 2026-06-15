using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.ContentSharing;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IPostService
    {
        Task<List<PostDto>> GetAllPostsAsync();
        Task<List<HalqaBriefDto>> GetHalqasBriefAsync(string? query = null);
        Task<GeneralResponse> PublishPostAsync(PublishPostRequest request);
        Task<GeneralResponse> UpdatePostAsync(UpdatePostRequest request);
        Task<GeneralResponse> DeletePostAsync(DeletePostRequest request);
        Task<GeneralResponse> GetPostByIdAsync(GetPostByIdRequest request);
        Task<GeneralResponse> GetPostInteractionsAsync(GetPostInteractionsRequest request);
        Task<GeneralResponse> InteractWithPostAsync(InteractWithPostRequest request);
        Task<GeneralResponse> ManageAnnouncementsAsync(ManageAnnouncementRequest request);
        Task<GeneralResponse> SearchContentPostAsync(SearchContentRequest request);
        Task<GeneralResponse> SearchContentGetAsync(string query);
        Task<GeneralResponse> DeleteOldContentAsync(DeleteOldContentRequest request);
        Task<GeneralResponse> AddMultimediaAsync(AddMultimediaRequest request);
    }
}
