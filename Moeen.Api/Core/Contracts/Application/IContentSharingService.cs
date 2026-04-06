using Moeen.Api.Shared.Requests.ContentSharing;
using Moeen.Api.Shared.Responses.ContentSharing;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IContentSharingService
    {
        /// <summary>
        /// نشر منشور عام أو للحلقات
        /// </summary>
        Task<PostDto> PublishPostAsync(PublishPostRequest request);

        /// <summary>
        /// التفاعل مع منشور (إعجاب، دعم، إلخ)
        /// </summary>
        Task<InteractWithPostResponse> InteractWithPostAsync(InteractWithPostRequest request);

        /// <summary>
        /// البحث في المحتوى
        /// </summary>
        Task<SearchContentResponse> SearchContentAsync(SearchContentRequest request);

        /// <summary>
        /// أرشفة المحتوى القديم
        /// </summary>
        Task<ArchiveOldContentResponse> ArchiveOldContentAsync(ArchiveOldContentRequest request);

        /// <summary>
        /// إدارة الإعلانات (إنشاء/تحديث)
        /// </summary>
        Task<ManageAnnouncementResponse> ManageAnnouncementsAsync(ManageAnnouncementRequest request);

        /// <summary>
        /// إضافة وسائط متعددة (صور) لمنشور
        /// </summary>
        Task<AddMultimediaResponse> AddMultimediaAsync(AddMultimediaRequest request);
    }
}