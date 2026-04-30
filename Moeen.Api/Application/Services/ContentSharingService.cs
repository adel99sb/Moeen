using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.ContentSharing;

namespace Moeen.Api.Application.Services
{
    public class ContentSharingService : IContentSharingService
    {
        public Task<AddMultimediaResponse> AddMultimediaAsync(AddMultimediaRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ArchiveOldContentResponse> ArchiveOldContentAsync(ArchiveOldContentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> DeletePostAsync(DeletePostRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PostDto> GetPostByIdAsync(GetPostByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<InteractionDto>> GetPostInteractionsAsync(GetPostInteractionsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<InteractWithPostResponse> InteractWithPostAsync(InteractWithPostRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ManageAnnouncementResponse> ManageAnnouncementsAsync(ManageAnnouncementRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PostDto> PublishPostAsync(PublishPostRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SearchContentResponse> SearchContentAsync(SearchContentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PostDto> UpdatePostAsync(UpdatePostRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
