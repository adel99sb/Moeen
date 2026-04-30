using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.ContentSharing;
using System.Collections.Generic;
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

        /// <summary>
        /// [GET] جلب منشور محدد بمعرفه مع تفاصيل التفاعلات
        /// </summary>
        Task<PostDto> GetPostByIdAsync(GetPostByIdRequest request);

        /// <summary>
        /// [GET] جلب قائمة التفاعلات على منشور معين
        /// </summary>
        Task<List<InteractionDto>> GetPostInteractionsAsync(GetPostInteractionsRequest request);

        /// <summary>
        /// [PUT] تحديث محتوى منشور موجود
        /// </summary>
        Task<PostDto> UpdatePostAsync(UpdatePostRequest request);

        /// <summary>
        /// [DELETE] حذف منشور نهائيًا
        /// </summary>
        Task<OperationResponseDto> DeletePostAsync(DeletePostRequest request);
    }
}